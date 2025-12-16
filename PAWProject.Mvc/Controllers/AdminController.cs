using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAW3CP1.Architecture;
using PAW3CP1.Architecture.Providers;
using PAWProject.Data.Models;
using PAWProject.Mvc.Models.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PAWProject.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IRestProvider _restProvider;
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _apiBaseUrl;

        public AdminController(ILogger<AdminController> logger, IRestProvider restProvider, IHttpClientFactory httpFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpFactory = httpFactory;
            _restProvider = restProvider;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7060";
        }

        #region Download/Upload/SaveToDB Article

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSource(CreateSourceDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["SourceError"] = "Datos inválidos. Revise el formulario.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var client = _httpFactory.CreateClient();
                client.BaseAddress = new Uri(_apiBaseUrl);

                var response = await client.PostAsJsonAsync("/api/source", dto);
                var body = await response.Content.ReadAsStringAsync();
                var requestUri = "api/source";
                _logger.LogInformation("Enviando POST a {FullUri}", new Uri(client.BaseAddress, requestUri));

                _logger.LogInformation("POST /api/source -> {Status}", response.StatusCode);
                _logger.LogDebug("POST /api/source body: {Body}", body);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SourceSuccess"] = "Fuente creada correctamente.";
                }
                else
                {
                    TempData["SourceError"] = $"Error creando fuente: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando fuente");
                TempData["SourceError"] = "Error interno al crear la fuente.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadJson(IFormFile jsonFile, int sourceId)
        {

            if (jsonFile == null || jsonFile.Length == 0)
            {
                TempData["UploadError"] = "Seleccione un archivo JSON válido.";
                return RedirectToAction("Index");
            }

            const long maxBytes = 5 * 1024 * 1024;
            if (jsonFile.Length > maxBytes)
            {
                TempData["UploadError"] = "El archivo excede el tamaño máximo permitido (5 MB).";
                return RedirectToAction("Index");
            }

            string jsonContent;
            try
            {
                using var sr = new StreamReader(jsonFile.OpenReadStream());
                jsonContent = await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leyendo el archivo JSON subido");
                TempData["UploadError"] = "No se pudo leer el archivo.";
                return RedirectToAction("Index");
            }
            

            try
            {
                //var dto = JsonProvider.DeserializeSimple<SourceDTO>(jsonContent); ** utilice este para poder subir el JSON a la tabla sourceItem 
                using var _ = System.Text.Json.JsonDocument.Parse(jsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JSON inválido subido por el usuario");
                TempData["UploadError"] = "El archivo no contiene JSON válido.";
                return RedirectToAction("Index");
            }

            try
            {
                // UTILICE REST PROVIDER, es mucha más facil... _restProvider
                var client = _httpFactory.CreateClient();
                client.BaseAddress = new Uri(_apiBaseUrl);

                var payload = new { json = jsonContent };
                var response = await client.PostAsJsonAsync("/api/sourceitems/upload-auto", payload);

                var respBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("POST /api/sourceitems/upload -> {Status}", response.StatusCode);
                _logger.LogDebug("POST /api/sourceitems/upload body: {Body}", respBody);

                if (response.IsSuccessStatusCode)
                    TempData["UploadSuccess"] = "Noticia cargada correctamente.";
                else
                    TempData["UploadError"] = $"Error al guardar: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reenviando noticia al API");
                TempData["UploadError"] = "Error interno al intentar guardar la noticia.";
            }

            return RedirectToAction("Index","Home");
        }

        #endregion

    }
}
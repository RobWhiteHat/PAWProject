using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAWProject.Mvc.Models.DTOs;

namespace PAWProject.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _apiBaseUrl;

        public AdminController(ILogger<AdminController> logger, IHttpClientFactory httpFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpFactory = httpFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7060";
        }

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
    }
}
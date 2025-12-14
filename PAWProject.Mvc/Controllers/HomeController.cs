using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAW3CP1.Architecture;
using PAW3CP1.Architecture.Providers;
using PAWProject.Data.Models;
using PAWProject.Models.DTO.SpaceFlightDTOs;
using PAWProject.Mvc.Models;



namespace PAWProject.Mvc.Controllers
{
    [Authorize(Roles = "Admin,Cliente,User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRestProvider _restProvider;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly IHttpClientFactory _httpFactory;



        public HomeController(ILogger<HomeController> logger, IRestProvider restProvider, IConfiguration configuration, IHttpClientFactory httpFactory)
        {
            _logger = logger;
            _restProvider = restProvider;
            _configuration = configuration;
            _httpFactory = httpFactory;

            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7060/api/SpaceApi";
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        #region PartialView Testing

        [HttpGet]
        public async Task<IActionResult> LoadArticlesPartial(int limit = 9, int offset = 0)
        {
            var endpoint = $"{_apiBaseUrl}/SpaceApi?limit={limit}&offset={offset}";
            var response = await _restProvider.GetAsync(endpoint, null);
            var articles = JsonProvider.DeserializeSimple<SpaceApiDTO>(response);

            var vm = articles.Results.Select(a => new ArticleViewModel
            {
                Url = a.Url,
                Title = a.Title,
                Description = a.Summary.Length > 120
                    ? a.Summary.Substring(0, 120) + "..."
                    : a.Summary,
                ImageUrl = a.ImageUrl,
                ComponentType = "API",
                RequiresSecret = false
            });

            return PartialView("_ArticleCard", vm);
        }

        [HttpGet]
        public async Task<IActionResult> LoadDbArticlesPartial()
        {

            var endpoint = $"{_apiBaseUrl}/Source";
            var response = await _restProvider.GetAsync(endpoint, null);
            var articlesDB = JsonProvider.DeserializeSimple<IEnumerable<SourceDTO>>(response);

            var vm = articlesDB.Select(s => new ArticleViewModel
            {
                Url = s.Url,
                Title = s.Name,
                Description = s.Description.Length > 120
                    ? s.Description.Substring(0, 120) + "..."
                    : s.Description,
                ComponentType = s.ComponentType,
                RequiresSecret = s.RequiresSecret
            });

            return PartialView("_DbArticleCard", vm);
        }

        #endregion

        #region Download/Upload/SaveToDB Article
        [Authorize(Roles = "Admin")]
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
                var client = _httpFactory.CreateClient();
                client.BaseAddress = new Uri(_apiBaseUrl);

                var payload = new { sourceId = sourceId, json = jsonContent };
                var response = await client.PostAsJsonAsync("/api/sourceitems/upload", payload);

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

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveArticleToDb([FromBody] SourceDTO dto)
        {
            try
            {
                var endpoint = $"{_apiBaseUrl}/Source";
                var json = JsonProvider.Serialize(dto);
                var response = await _restProvider.PostAsync(endpoint, json);
                TempData["UploadSuccess"] = "Fuente guardada con éxito";
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = $"Error loading notifications: {ex.Message}";
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Old Loading - JavaScript

        [HttpGet]
        public async Task<IActionResult> LoadArticles(int limit = 10, int offset = 0)
        {
            var endpoint = $"{_apiBaseUrl}/SpaceApi?limit={limit}&offset={offset}";
            var response = await _restProvider.GetAsync(endpoint, null);
            var articles = JsonProvider.DeserializeSimple<SpaceApiDTO>(response);

            return Json(articles);
        }

        [HttpGet]
        public async Task<IActionResult> LoadDbArticles()
        {
            var endpoint = $"{_apiBaseUrl}/Source";
            var response = await _restProvider.GetAsync(endpoint, null);
            var articlesDB = JsonProvider.DeserializeSimple<IEnumerable<SourceDTO>>(response);

            bool isAdmin = User.IsInRole("Admin");

            var filtered = isAdmin
                ? articlesDB
                : articlesDB.Where(a => !a.RequiresSecret);

            return Json(filtered);
        }

        #endregion

        #region Others
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}

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

        #region PartialView

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
                Description = a.Summary,
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

            bool isAdmin = User.IsInRole("Admin");

            var filtered = isAdmin
                ? articlesDB
                : articlesDB.Where(a => !a.RequiresSecret);

            var vm = filtered.Select(s => new ArticleViewModel
            {
                Url = s.Url,
                Title = s.Name,
                Description = s.Description,
                ComponentType = s.ComponentType,
                RequiresSecret = s.RequiresSecret
            });

            return PartialView("_DbArticleCard", vm);
        }

        #endregion

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

        #region Old Loading - JavaScript
        /*
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
        }*/

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

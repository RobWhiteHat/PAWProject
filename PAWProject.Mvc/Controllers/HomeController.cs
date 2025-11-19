using System.Configuration;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAW3CP1.Architecture;
using PAW3CP1.Architecture.Providers;
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


        public HomeController(ILogger<HomeController> logger, IRestProvider restProvider, IConfiguration configuration)
        {
            _logger = logger;
            _restProvider = restProvider;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7060/api/SpaceApi";
        }

        public async Task<IActionResult> Index()
        {
            var endpoint = $"{_apiBaseUrl}/SpaceApi";
            var response = await _restProvider.GetAsync(endpoint, null);
            var spaceArticles = JsonProvider.DeserializeSimple<SpaceApiDTO>(response) ?? new SpaceApiDTO();
            return View(spaceArticles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

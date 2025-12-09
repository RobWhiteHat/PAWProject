using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Json;
using PAWProject.Mvc.Models;

namespace PAWProject.Mvc.Controllers
{
    public class SourceItemController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public SourceItemController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7060"); // ajusta puerto del API
            var items = await client.GetFromJsonAsync<List<SourceItemDtoForUi>>("/api/sourceitems");
            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View(new SourceItemUploadViewModel());

        [HttpPost]
        public async Task<IActionResult> Upload(SourceItemUploadViewModel model)
        {
            if (!ModelState.IsValid) return View("Create", model);

            if (model.JsonFile == null || model.JsonFile.Length == 0)
            {
                ModelState.AddModelError("", "Archivo vacío");
                return View("Create", model);
            }

            string jsonContent;
            using (var reader = new StreamReader(model.JsonFile.OpenReadStream()))
                jsonContent = await reader.ReadToEndAsync();

            try { JsonDocument.Parse(jsonContent); }
            catch (JsonException)
            {
                ModelState.AddModelError("", "JSON inválido");
                return View("Create", model);
            }

            var client = _httpFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7060"); // ajusta puerto del API
            var payload = new { sourceId = model.SourceId, json = jsonContent };
            var response = await client.PostAsJsonAsync("/api/sourceitems/upload", payload);

            ViewBag.Message = response.IsSuccessStatusCode ? "Noticia subida correctamente" : "Error al guardar la noticia";
            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7060");
            var item = await client.GetFromJsonAsync<SourceItemDtoForUi>($"/api/sourceitems/{id}");
            if (item == null) return NotFound();

            return View(item);
        }
    }

    // este seria el  DTO para consumir el API , si tienen dudas me avisan
    public class SourceItemDtoForUi
    {
        public int Id { get; set; }
        public int? SourceId { get; set; }
        public string? Json { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
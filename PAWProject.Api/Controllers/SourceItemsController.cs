using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAWProject.Api.Models.DTO;
using PAWProject.Core.Interfaces;
using PAWProject.Data.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace PAWProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceItemsController : ControllerBase
    {
        private readonly ISourceItemService _service;
        private readonly ILogger<SourceItemsController> _logger;

        public SourceItemsController(ISourceItemService service, ILogger<SourceItemsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST api/sourceitems/upload
        [HttpPost("upload")]
        //[Authorize(Roles = "Admin,Editor")] // habilitar cuando la auth esté lista
        public async Task<IActionResult> Upload([FromBody] SourceItemDto dto)
        {
            if (dto == null || dto.SourceId <= 0 || string.IsNullOrWhiteSpace(dto.Json))
                return BadRequest("Payload inválido");

            // Validar que el string sea JSON válido
            try { JsonDocument.Parse(dto.Json); }
            catch (JsonException) { return BadRequest("JSON inválido"); }

            var entity = new SourceItem
            {
                SourceId = dto.SourceId,
                Json = dto.Json,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var success = await _service.SaveItemAsync(entity);
                if (!success) return BadRequest("No se pudo guardar el item (verifica SourceId)");
                return Ok(new { message = "Item guardado correctamente", id = entity.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar SourceItem");
                return StatusCode(500, "Error interno al guardar el item");
            }
        }

        // GET api/sourceitems
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // GET api/sourceitems/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}

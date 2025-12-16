using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAWProject.Api.Models.DTO;
using PAWProject.Core;
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
        private readonly ISourceService _sourceService;
        private readonly ILogger<SourceItemsController> _logger;

        public SourceItemsController(
            ISourceItemService service,
            ISourceService sourceService,
            ILogger<SourceItemsController> logger)
        {
            _service = service;
            _sourceService = sourceService;
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
        // POST api/sourceitems/upload-auto
        [HttpPost("upload-auto")]
        public async Task<IActionResult> UploadAuto([FromBody] JsonUploadDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Json))
                return BadRequest("JSON vacío");

            // 1. Validar que el JSON sea válido
            try { JsonDocument.Parse(dto.Json); }
            catch { return BadRequest("JSON inválido"); }

            // 2. Deserializar el JSON a un DTO que representa una Source
            SourceFromJsonDto sourceDto;
            try
            {
                sourceDto = JsonSerializer.Deserialize<SourceFromJsonDto>(
                    dto.Json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch
            {
                return BadRequest("El JSON no coincide con el formato esperado.");
            }


            // 3. Buscar si ya existe una fuente con la misma URL
            var existing = await _sourceService.GetByUrlAsync(sourceDto.Url);

            Source source;
            if (existing != null)
            {
                source = existing;
            }
            else
            {
                // 4. Crear una nueva fuente automáticamente
                source = new Source
                {
                    Url = sourceDto.Url,
                    Name = sourceDto.Name,
                    Description = sourceDto.Description,
                    ComponentType = sourceDto.ComponentType,
                    RequiresSecret = sourceDto.RequiresSecret == 1
                };

                await _sourceService.CreateSourceAsync(source);
            }

            // 5. Crear el SourceItem asociado a esa fuente
            var item = new SourceItem
            {
                SourceId = source.Id,
                Json = dto.Json,
                CreatedAt = DateTime.UtcNow
            };

            var success = await _service.SaveItemAsync(item);
            if (!success)
                return StatusCode(500, "No se pudo guardar el SourceItem");

            return Created("", new
            {
                message = "Source y SourceItem creados correctamente",
                sourceId = source.Id,
                itemId = item.Id
            });
        }
    }
}

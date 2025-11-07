using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PAWProject.Core.Interfaces;
using PAWProject.Data.Models;

namespace PAWProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceItemsController : ControllerBase
    {
        private readonly ISourceItemService _service;

        public SourceItemsController(ISourceItemService service)
        {
            _service = service;
        }

        [HttpPost]
        //[Authorize] desactivado temporalmente para pruebas
        public async Task<IActionResult> SaveItem([FromBody] SourceItem item)
        {
            var success = await _service.SaveItemAsync(item);
            if (!success) return BadRequest("SourceId inválido");
            return Ok("Item guardado correctamente");
        }
    }
}
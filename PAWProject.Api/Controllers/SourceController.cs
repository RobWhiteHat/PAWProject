using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAWProject.Core;
using PAWProject.Data.Models;
using PAWProject.Api.Models.DTO; 

namespace PAWProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class SourceController : ControllerBase
    {
        private readonly ISourceService _sourceService;

        public SourceController(ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        // GET: api/source
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Source>>> Get()
        {
            var sources = await _sourceService.GetArticlesFromDBAsync(id: null);
            return Ok(sources);
        }

        // GET: api/source/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Source>> GetById(int id)
        {
            var source = await _sourceService.GetByIdAsync(id);
            if (source == null) return NotFound();
            return Ok(source);
        }

        // POST: api/source
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateSourceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Source
            {
                Name = dto.Name,
                Url = dto.Url,
                Description = dto.Description,
                ComponentType = dto.ComponentType,
                RequiresSecret = dto.RequiresSecret
            };

            var created = await _sourceService.CreateSourceAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
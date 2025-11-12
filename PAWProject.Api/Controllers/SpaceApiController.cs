using Microsoft.AspNetCore.Mvc;
using PAWProject.Api.Services.Contracts;
using PAWProject.Models.DTO.SpaceFlightDTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PAWProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceApiController : ControllerBase
    {

        private readonly ISpaceService _spaceService;

        public SpaceApiController(ISpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        // GET: api/<SpaceApiController>
        [HttpGet]
        public async Task<ActionResult<SpaceApiDTO>> GetAsync()
        {
            var space = await _spaceService.GetDataAsync();

            if (space == null)
                return NotFound("No se encontraron artículos.");

            return Ok(space);
        }



        /*
        // GET api/<SpaceApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SpaceApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SpaceApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SpaceApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}

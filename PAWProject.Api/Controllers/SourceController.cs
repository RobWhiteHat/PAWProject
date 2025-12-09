using Microsoft.AspNetCore.Mvc;
using PAWProject.Core;
using PAWProject.Data.Models;

namespace PAWProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceController(ISourceService sourceService) : ControllerBase
    {
        // GET: api/UserApiController
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Source>>> Get()

        {
            var sources = await sourceService.GetArticlesFromDBAsync(id: null);
            return Ok(sources);
        }
    }
}

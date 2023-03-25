using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bugtrackerback.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [HttpPost("projects"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProjects()
        {
            return Ok("Super secret hello");
        }
    }
}

using bugtrackerback.Areas.Identity.Data;
using bugtrackerback.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bugtrackerback.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly bugtrackerdbContext BugtrackerdbContext;
        public ProjectController(bugtrackerdbContext bugtrackerdbContext)
        {
            BugtrackerdbContext = bugtrackerdbContext;
        }

        
        [HttpPost("projects1"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProjects1()
        {
            return Ok("result of request");
        }

        [HttpPost("projects")]
        public async Task<List<Project>> GetProjects()
        {
            return await BugtrackerdbContext.Projects.ToListAsync();
        }
    }
}

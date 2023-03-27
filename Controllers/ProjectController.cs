using bugtrackerback.Areas.Identity.Data;
using bugtrackerback.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;

namespace bugtrackerback.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly bugtrackerdbContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectController(bugtrackerdbContext bugtrackerdbContext, UserManager<User> userManager)
        {
            _context = bugtrackerdbContext;
            _userManager = userManager;
        }

        
        [HttpPost("projects1"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProjects1()
        {
            return Ok("result of request");
        }

        [HttpPost("projects")]
        public async Task<List<Project>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }


        [HttpPost("createProject")]
        public async Task<IActionResult> CreateProject(ProjectCreationDTO projectData)
        {
            User currentUser = await _userManager.FindByEmailAsync(projectData.UserEmail);
            List<User> assignedUserList = new List<User>();
            User? foundUser = null;
            if(projectData.AssignedUserEmails != null)
            {
                foreach (string email in projectData.AssignedUserEmails)
                {
                    foundUser = await _userManager.FindByEmailAsync(email);
                    assignedUserList.Add(foundUser);
                }
            }


            Project newProject = new Project
            {
                Name = projectData.Name,
                Description = projectData.Description,
                Author = currentUser,
                Users = assignedUserList
            };

            var result = await _context.Projects.AddAsync(newProject);

            return Ok(result);
        }
    }
}

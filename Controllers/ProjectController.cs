using bugtrackerback.Areas.Identity.Data;
using bugtrackerback.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace bugtrackerback.Controllers
{
    // [Authorize]
    [Route("api/[controller]/[action]")]
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

        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectCreationDTO projectData)
        {
            User currentUser = await _userManager.FindByEmailAsync(projectData.UserEmail);
            List<User> assignedUserList = new List<User>();
            User? foundUser = null;
            if(projectData.AssignedUserEmails != null && projectData.AssignedUserEmails.Count > 0)
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
                AuthorId = currentUser.Id,
                Users = assignedUserList
            };

            var result = await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();

            return Ok("Project created succesfully");
        }

        [HttpPost]
        public async Task<IActionResult> GetProjectData()
        { 
            var projectData = await _context.Projects.Select(p => new { p.Id, p.Name, p.Description }).ToListAsync();

            return Ok(projectData);
        }

        [HttpPost]
        public async Task<IActionResult> GetAssignedUsers(string projectId)
        {
            var usersInProject = _context.Projects.Where(p => p.Id.ToString() == projectId)
                .SelectMany(c => c.Users).Select(u => new { u.Id, u.Email, u.Name, u.Surname });

            return Ok(usersInProject);
        }
    }
}

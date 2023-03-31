using bugtrackerback.Areas.Identity.Data;
using bugtrackerback.Entities;
using bugtrackerback.Entities.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bugtrackerback.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly bugtrackerdbContext _context;

        public TicketController(bugtrackerdbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDTO createTicketDTO)
        {
            var ticketAuthor = _context.Users.FirstOrDefault(u => u.Email == createTicketDTO.AuthorEmail);
            var project = _context.Projects.FirstOrDefault(p => p.Id == createTicketDTO.ProjectId);
            var assignedUsers = await _context.Users.Where(u => createTicketDTO.UserIds.Contains(u.Id)).ToListAsync();

            if(ticketAuthor != null && project != null)
            {
                Ticket ticketResult = new Ticket()
                {
                    Title = createTicketDTO.Name,
                    Description = createTicketDTO.Description,
                    Priority = createTicketDTO.Priority,
                    Type = createTicketDTO.Type,
                    Status = createTicketDTO.Status,
                    AuthorId = ticketAuthor.Id,
                    Comments = null,
                    Users = assignedUsers,
                    Project = project
                };

                var result = await _context.Tickets.AddAsync(ticketResult);
                _context.SaveChanges();

                return Ok("Success!");
            }


            return BadRequest("Failed to create ticket");
        }
    }
}

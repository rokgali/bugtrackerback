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

        [HttpGet]
        public async Task<IActionResult> GetAssignedUsers(string ticketId)
        {
            var foundTicket = await _context.Tickets.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == ticketId);
            List<User> ticketUsers = new List<User>();

            if(foundTicket != null)
            {
                foreach(User user in foundTicket.Users)
                {
                    ticketUsers.Add(user);
                }

                var returnUserData = ticketUsers.Select(u => new { u.Id, u.Email, u.Name, u.Surname }).ToList(); 
                return Ok(returnUserData);
            }

            return BadRequest("Ticket does not exist");
        }
        [HttpPost]
        public async Task<IActionResult> WriteComment(CommentDTO commentDTO)
        {
            var ticket = await _context.Tickets.FirstAsync(t => t.Id == commentDTO.TicketId);

            if(ticket != null)
            {
                var author = await _context.Users.FirstAsync(u => u.Email == commentDTO.AuthorEmail);

                TicketComment comment = new TicketComment()
                {
                    Comment = commentDTO.Comment,
                    Author = author,
                    Ticket = ticket
                };

                await _context.TicketComments.AddAsync(comment);
                await _context.SaveChangesAsync();

                return Ok("Ticket succesfully commented");
            }

            return BadRequest("Ticket does not exist");
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(string ticketId)
        {
            var ticket = await _context.Tickets.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == ticketId);
            List<TicketComment> comments = new List<TicketComment>();

            if(ticket.Comments != null)
            {
                var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == ticket.AuthorId);

                foreach (TicketComment comment in ticket.Comments)
                {
                    comments.Add(comment);
                }

                var commentData = comments.Select(c => new {c.Id, c.DateTime, c.Comment, author.Name, author.Surname, author.Email});
                return Ok(commentData);
            }

            return BadRequest("This ticket does not exist");
        }
    }
}

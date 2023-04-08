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
            var comments = await _context.TicketComments.Include(c => c.Author)
                .Where(c => c.Ticket.Id == ticketId)
                .ToListAsync();

            var returnData = comments.Select(d => new { d.Id, d.DateTime, d.Comment, d.Author.Name, d.Author.Surname, d.Author.Email });

            return Ok(returnData);
        }

        [HttpPost]
        public async Task<IActionResult> EditTicket(EditTicketDTO editTicketDTO)
        {
            var ticketToEdit = await _context.Tickets.Include(t => t.Users).Where(t => t.Id == editTicketDTO.Id).FirstAsync();

            if(ticketToEdit != null)
            {
                var userList = await _context.Users.Where(u => editTicketDTO.UserIds.Contains(u.Id)).ToListAsync();
     
                ticketToEdit.Title = editTicketDTO.Title;
                ticketToEdit.Description = editTicketDTO.Description;
                ticketToEdit.Priority = editTicketDTO.Priority;
                ticketToEdit.Type = editTicketDTO.Type;
                ticketToEdit.Status = editTicketDTO.Status;
                ticketToEdit.Users = userList;

                await _context.SaveChangesAsync();

                return Ok("Ticket edited succesfully");
            }

            return BadRequest("The ticket does not exist");
        }
    }
}

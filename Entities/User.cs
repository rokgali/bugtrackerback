using Microsoft.AspNetCore.Identity;

namespace bugtrackerback.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public List<Project>? Projects { get; set; }
        public List<Ticket>? Tickets { get; set; }
        public List<TicketComment>? ticketComments { get; set; }
    }
}

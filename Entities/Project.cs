using System.ComponentModel.DataAnnotations.Schema;

namespace bugtrackerback.Entities
{
    public class Project
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<User>? Users { get; set; }
        public string AuthorId { get; set; } = null!;
        public List<Ticket>? Tickets { get; set; }
    }
}

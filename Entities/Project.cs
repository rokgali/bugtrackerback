using System.ComponentModel.DataAnnotations.Schema;

namespace bugtrackerback.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<User>? Users { get; set; }
        [NotMapped]
        public User Author { get; set; } = null!;
        public List<Ticket>? Tickets { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace bugtrackerback.Entities
{
    public enum Priority
    {
        high,
        medium,
        low
    };

    public enum Type
    {
        bug,
        feature,
        other
    };

    public enum Status
    {
        unadressed,
        in_progress,
        resolved
    };
    public class Ticket
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Priority Priority { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public string AuthorId { get; set; } = null!;
        public List<TicketComment>? Comments { get; set; }
        public List<User>? Users { get; set; }
        public Project Project { get; set; } = null!;
    }
}

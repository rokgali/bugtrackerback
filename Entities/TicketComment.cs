namespace bugtrackerback.Entities
{
    public class TicketComment
    {
        public Guid Id { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public User Author { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}

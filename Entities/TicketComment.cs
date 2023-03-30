namespace bugtrackerback.Entities
{
    public class TicketComment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Comment { get; set; } = null!;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public User Author { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}

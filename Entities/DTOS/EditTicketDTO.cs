namespace bugtrackerback.Entities.DTOS
{
    public class EditTicketDTO
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Priority Priority { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public List<string>? UserIds { get; set; }
    }
}

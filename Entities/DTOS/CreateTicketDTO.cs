namespace bugtrackerback.Entities.DTOS
{
    public class CreateTicketDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Priority Priority { get; set; }
        public Type Type { get; set; }
        public Status Status { get; set; }
        public string AuthorEmail { get; set; } = null!;
        public List<string>? UserIds { get; set; }
        public string ProjectId { get; set; } = null!;
    }
}

namespace bugtrackerback.Entities.DTOS
{
    public class CommentDTO
    {
        public string Comment { get; set; } = null!;
        public string AuthorEmail { get; set; } = null!;
        public string TicketId { get; set; } = null!;
    }
}

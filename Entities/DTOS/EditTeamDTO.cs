namespace bugtrackerback.Entities.DTOS
{
    public class EditTeamDTO
    {
        public string ProjectId { get; set; } = null!;
        public List<string> UsersIds { get; set; } = null!;
    }
}

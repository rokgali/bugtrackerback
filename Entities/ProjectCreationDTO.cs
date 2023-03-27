﻿namespace bugtrackerback.Entities
{
    public class ProjectCreationDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public List<string>? AssignedUserEmails { get; set; }
    }
}

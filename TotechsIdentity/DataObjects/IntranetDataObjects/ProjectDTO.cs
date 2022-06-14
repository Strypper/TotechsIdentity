using System;
using System.Collections.Generic;

namespace TotechsIdentity.DataObjects.IntranetDataObjects
{
    public class ProjectDTO : BaseDTO
    {
        public string ProjectName { get; set; } = "UnTitle";
        public string? ProjectLogo { get; set; } = string.Empty;
        public string? ProjectBackground { get; set; } = string.Empty;
        public string? Clients { get; set; } = String.Empty;
        public string? About { get; set; } = String.Empty;
        public string? GithubLink { get; set; } = string.Empty;
        public string? FigmaLink { get; set; } = string.Empty;
        public string? MicrosoftStoreLink { get; set; } = string.Empty;
        public string? GooglePlayLink { get; set; } = string.Empty;
        public string? AppStoreLink { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public int TechLead { get; set; }
    }

    public class ProjectWithMemberDTO : ProjectDTO
    {
        public IEnumerable<UserDTO> Members { get; set; } = new HashSet<UserDTO>();
    }
}

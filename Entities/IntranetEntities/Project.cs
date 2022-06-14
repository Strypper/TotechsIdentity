using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IntranetEntities
{
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; }
        public string? ProjectLogo { get; set; } = string.Empty;
        public string? ProjectBackground { get; set; } = string.Empty;
        public string? Clients { get; set; } = string.Empty;
        public string? About { get; set; } = string.Empty;
        public string? GithubLink { get; set; } = string.Empty;
        public string? FigmaLink { get; set; } = string.Empty;
        public string? MicrosoftStoreLink { get; set; } = string.Empty;
        public string? GooglePlayLink { get; set; } = string.Empty;
        public string? AppStoreLink { get; set; } = string.Empty;


        public DateTime StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public int TechLead { get; set; }

    }
}

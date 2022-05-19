using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TotechsIdentity.DataObjects
{
    public class RoleDTO
    {
        [Required]
        public string Id         { get; set; }
        [Required]
        public string  Name      { get; set; } = string.Empty;
        public string? RoleIcon  { get; set; }
        public string  Summary   { get; set; }
        public string  Mission   { get; set; }
        public string  MainTasks { get; set; }

        public ICollection<RoleLevelDTO>? RoleLevels { get; set; } = new HashSet<RoleLevelDTO>();
    }
}

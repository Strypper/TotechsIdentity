using System.ComponentModel.DataAnnotations;

namespace TotechsIdentity.DataObjects
{
    public class RoleDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Entities
{
    public class Role : IdentityRole
    {
        public string RoleIcon    { get; set; }
        public string Summary     { get; set; }
        public string Mission     { get; set; }
        public string MainTasks   { get; set; }

        public ICollection<RoleLevel> RoleLevels { get; set; } = new HashSet<RoleLevel>();
        public virtual ICollection<UserRole> UserRoles { get; } = new HashSet<UserRole>();
    }
}

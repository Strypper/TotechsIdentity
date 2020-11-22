using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class User : IdentityUser
    {
        public string Guid { get; protected set; } = null!;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;

        public int? SmartZoneId { get; set; }
        public DateTime DateJoin { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsExpired { get; set; } = false;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}

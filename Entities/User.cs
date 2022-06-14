using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class User : IdentityUser
    {
        public string    Guid                            { get; protected set; } = null!;
        public string    FirstName                       { get; set; }           = String.Empty;
        public string?   MiddleName                      { get; set; }
        public string    LastName                        { get; set; } = String.Empty;
        public bool      IsDeleted                       { get; set; } = false;
        public bool      IsExpired                       { get; set; } = false;
        public bool?     Gender                          { get; set; }
        public DateTime  DateJoin                        { get; set; }
        public DateTime? DateOfBirth                     { get; set; }
        public Country?  Country                         { get; set; }
        public string?   ProfilePicUrl                   { get; set; }
        public virtual   ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}

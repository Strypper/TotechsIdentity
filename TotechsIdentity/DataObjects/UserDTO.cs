using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TotechsIdentity.DataObjects
{
    public class UserDTO
    {
        [FromRoute]
        public string Guid { get; set; }
        [Required]
        public string UserName { get; set; } = String.Empty;
        [Required]
        public string FirstName { get; set; } = String.Empty;
        [Required]
        public string LastName { get; set; } = String.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        [Required]
        public string PhoneNumber { get; set; } = String.Empty;
        [Required]
        public string SmartZoneGuid { get; set; } = String.Empty;
        public ICollection<string> Roles { get; set; } = Array.Empty<string>();
    }
}

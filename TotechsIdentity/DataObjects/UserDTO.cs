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
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? ProfilePicUrl { get; set; }
        public string? ProfilePicName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public CountryDTO Country { get; set; }
        public ICollection<string> Roles { get; set; } = Array.Empty<string>();
    }

    public class CreateUserDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        // public int RequestServiceId { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? ProfilePicName { get; set; }
        public ICollection<string> Roles { get; set; } = Array.Empty<string>();
    }
}

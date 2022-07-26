namespace TotechsIdentity.Models
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class PhoneNumberLogin
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}

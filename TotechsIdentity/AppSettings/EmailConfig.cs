namespace TotechsIdentity.AppSettings
{
    public class EmailConfig
    {
        public string UserName { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }
}

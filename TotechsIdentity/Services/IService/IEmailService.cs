using System.Threading.Tasks;

namespace TotechsIdentity.Services.IService
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string emailContent);
        Task SendEmailConfirmation(string confirmUrl, string username, string to);
    }
}

using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;
using TotechsIdentity.AppSettings;
using TotechsIdentity.Constants;
using TotechsIdentity.EmailTemplates;
using TotechsIdentity.Services.IService;

namespace TotechsIdentity.Services
{
    public class SMTPEmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly SmtpClient  _smtpClient;

        public SMTPEmailService(IOptionsMonitor<EmailConfig> emailConfig, 
                                SmtpClient smtpClient)
        {
            _emailConfig = emailConfig.CurrentValue;
            _smtpClient  = smtpClient;
        }

        public async Task SendEmail(string to, string subject, string emailContent)
        {
            MailAddress from = new(_emailConfig.UserName, _emailConfig.DisplayName);
            MailMessage message = new(_emailConfig.UserName, to, subject, emailContent);
            message.From = from;
            message.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(message);
        }

        public async Task SendEmailConfirmation(string confirmUrl, string username, string to)
        {
            var template = HtmlTemplates.EmailConfirmation;
            template = template.Replace("{{username}}", username).Replace("{{confirmUrl}}", confirmUrl);

            await SendEmail(to, EmailConstants.EmailConfirmation, template);
        }

        public async Task SendEmailResetPassword(string resetPassword, string username, string to)
        {
            var template = HtmlTemplates.EmailConfirmation;
            template = template.Replace("{{username}}", username).Replace("{{confirmUrl}}", resetPassword);

            await SendEmail(to, EmailConstants.EmailConfirmation, template);
        }
    }
}

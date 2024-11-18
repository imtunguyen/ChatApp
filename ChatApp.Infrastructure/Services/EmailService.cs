using ChatApp.Application.DTOs.Email;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ChatApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        EmailConfig _emailConfig;
        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendMailAsync(CancellationToken cancellationToken, EmailRequest emailRequest)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_emailConfig.Provider, _emailConfig.Port)
                {
                    Credentials = new NetworkCredential(_emailConfig.DefaultSender, _emailConfig.Password),
                    EnableSsl = _emailConfig.Equals("true")
                };
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailConfig.DefaultSender),
                    To = { emailRequest.To },
                    Subject = emailRequest.Subject,
                    Body = emailRequest.Content,
                    IsBodyHtml = true
                };
                await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                mailMessage.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

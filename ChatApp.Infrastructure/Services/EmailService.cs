using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Email;
using ChatApp.Domain.Exceptions;
using ChatApp.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ChatApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendMailAsync(CancellationToken cancellationToken, EmailRequest emailRequest)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_emailConfig.Provider, _emailConfig.Port);
                smtpClient.Credentials = new NetworkCredential(_emailConfig.DefaultSender, _emailConfig.Password);
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_emailConfig.DefaultSender);
                message.To.Add(emailRequest.To);
                message.Subject = emailRequest.Subject;
                message.Body = emailRequest.Content;
                message.IsBodyHtml = true;

                await smtpClient.SendMailAsync(message, cancellationToken);
                message.Dispose();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString());
            }

        }
    }
}

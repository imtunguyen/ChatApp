using ChatApp.Application.DTOs.Email;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IEmailService
    {
        Task SendMailAsync(CancellationToken cancellationToken, EmailRequest emailRequest);
    }
}

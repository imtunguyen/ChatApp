using ChatApp.Application.DTOs.Email;

namespace ChatApp.Application.Abstracts.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(CancellationToken cancellationToken, EmailRequest emailRequest);
    }
}

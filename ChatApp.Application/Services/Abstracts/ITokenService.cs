using ChatApp.Domain.Entities;

namespace ChatApp.Application.Services.Abstracts
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        Task<string> RefreshToken();
    }
}

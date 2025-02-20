using ChatApp.Domain.Entities;
using System.Security.Claims;

namespace ChatApp.Application.Abstracts.Services
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

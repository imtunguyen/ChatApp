
using ChatApp.Application.Abstracts.Services;
using ChatApp.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfig _config;
        public TokenService(IOptions<TokenConfig> config)
        {
            _config = config.Value;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            // Kiểm tra khóa bí mật có đủ mạnh không
            if (string.IsNullOrWhiteSpace(_config.SecretKey) || _config.SecretKey.Length < 32)
            {
                throw new ArgumentException("Secret key must be at least 32 characters long.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.ExpireMin), // Dùng UtcNow thay vì Now
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_config.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // Kiểm tra Issuer
                ValidateAudience = true, // Kiểm tra Audience
                ValidateLifetime = false, // Không kiểm tra hạn token (vì token đã hết hạn)
                ValidateIssuerSigningKey = true, // Kiểm tra chữ ký
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Đảm bảo kiểm tra chính xác thời gian hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return claimsPrincipal;
        }

    }
}

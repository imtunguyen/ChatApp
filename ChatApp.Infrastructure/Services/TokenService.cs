﻿using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]!));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                // Lấy thêm các quyền (claims) từ role
                var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role));
                claims.AddRange(roleClaims);
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = _config["Token:Issuer"],
                Audience = _config["Token:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
        public async Task<string> RefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        } 
    } 
}

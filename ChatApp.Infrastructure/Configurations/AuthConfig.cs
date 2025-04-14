using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Configuration;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Infrastructure.Configurations
{
    public static class AuthConfig
    {
        public static void AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<ChatAppContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication setup
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var jwtKey = configuration["TokenConfig:SecretKey"];
                 if (string.IsNullOrEmpty(jwtKey))
                 {
                     throw new ArgumentNullException("Jwt:Key", "JWT Key cannot be null or empty.");
                 }

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = configuration["TokenConfig:Issuer"],
                     ValidAudience = configuration["TokenConfig:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                 };
             
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var accessToken = context.Request.Query["access_token"];
                         var path = context.HttpContext.Request.Path;

                         Console.WriteLine($"🔍 Incoming SignalR path: {path}");
                         Console.WriteLine($"🔐 AccessToken received: {accessToken}");

                         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                         {
                             context.Token = accessToken;
                         }
                         return Task.CompletedTask;
                     },
                     OnTokenValidated = context =>
                     {
                         var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                         Console.WriteLine($"Token validated: UserId is {userId}");
                         return Task.CompletedTask;
                     }
                 };
             });

        }
    }
}

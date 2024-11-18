using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
                .AddEntityFrameworkStores<ChatAppContext>();

            // JWT Authentication setup
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]!)),
                        ValidIssuer = configuration["Token:Issuer"],
                        ValidAudience = configuration["Token:Audience"],
                        ValidateIssuer = true,
                        ValidateAudience = false
                    };
                });
        }
    }
}

using ChatApp.Application.Abstracts.Services;
using ChatApp.Infrastructure.Configuration;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace ChatApp.Infrastructure.Configurations
{
    public static class AppConfig
    {
        public static void AddAppConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinaryConfig>(configuration.GetSection("CloudinarySettings"));
            services.Configure<EmailConfig>(configuration.GetSection("MailSettings"));
            services.Configure<TokenConfig>(configuration.GetSection("TokenConfig"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
           

            // Configure token expiration for ASP.NET Identity tokens
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5); // 5 minutes
            });
        }
    }
}

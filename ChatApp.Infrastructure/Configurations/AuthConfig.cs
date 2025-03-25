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
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    TokenConfig token = new TokenConfig();
                    configuration.GetSection(nameof(TokenConfig)).Bind(token);
                    var key = Encoding.UTF8.GetBytes(token.SecretKey);
                    
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true, 
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = token.Issuer,
                        ValidAudience = token.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero,
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("IS-TOKEN-EXPRIED", "true");
                            }
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}

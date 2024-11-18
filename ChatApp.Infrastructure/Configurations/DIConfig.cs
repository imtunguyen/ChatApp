using ChatApp.Application.Services.Abstracts;
using ChatApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure.Configurations
{
    public static class DIConfig
    {
        public static void AddDIConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
        }
    }
}

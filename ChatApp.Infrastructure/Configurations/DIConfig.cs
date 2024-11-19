using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Services.Implementations;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure.Configurations
{
    public static class DIConfig
    {
        public static void AddDIConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSignalR();
        }
    }
}

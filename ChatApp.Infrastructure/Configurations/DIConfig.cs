using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Group;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Services.Implementations;
using ChatApp.Application.Validators;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ChatApp.Infrastructure.Configurations
{
    public static class DIConfig
    {
        public static void AddDIConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMessageService, MessageService>(); 
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IFriendShipService, FriendShipService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IValidator<GroupAddDto>, GroupValidator>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
            services.AddScoped<IUserStatusService, UserStatusService>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddSignalR();
        }
    }
}

﻿using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Configurations;
using ChatApp.Infrastructure.DataAccess.SeedData;
using ChatApp.Presentation.Middleware;
using ChatApp.Presentation.SignalR;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Database Configuration
builder.Services.AddDbConfig(builder.Configuration);

// Add Authentication Configuration
builder.Services.AddAuthConfig(builder.Configuration);

// Add Dependency Injection for Services
builder.Services.AddDIConfig(builder.Configuration);

// Add Application Settings Configuration
builder.Services.AddAppConfig(builder.Configuration);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("https://localhost:4200", "http://localhost:4200"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    await RoleSeed.SeedAsync(roleManager); 
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseDefaultFiles();
app.MapControllers();

app.MapHub<ChatHub>("/chat");
app.MapHub<WebRTCHub>("/webrtc");
app.Map("/", () => Results.Redirect("/swagger"));
app.Run();

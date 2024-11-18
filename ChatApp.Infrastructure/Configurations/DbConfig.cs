using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ChatApp.Infrastructure.Configurations
{
    public static class DbConfig
    {
        public static void AddDbConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            // SQL Server connection
            services.AddDbContext<ChatAppContext>(options => options.UseSqlServer(connectionString));
        }
    }
}

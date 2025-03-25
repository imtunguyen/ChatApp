using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.DataAccess.SeedData
{
    public class RoleSeed
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            if (await roleManager.Roles.AnyAsync()) return;

            var roles = new List<AppRole>
            {
                new() { Name = "GroupOwner", Description = "GroupOwner Role" },
                new() { Name = "Member", Description = "Member User" },
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}

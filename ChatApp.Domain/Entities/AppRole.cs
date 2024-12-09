using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    public class AppRole : IdentityRole
    {
        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}

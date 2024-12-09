using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public GenderType Gender { get; set; }
        public UserStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastActiveAt { get; set; } 
        public DateTimeOffset UpdatedAt { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}

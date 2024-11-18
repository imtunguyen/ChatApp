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
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}

using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public GenderType Gender { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<FriendShip> SentFriendRequests { get; set; } = new List<FriendShip>();
        public virtual ICollection<FriendShip> ReceivedFriendRequests { get; set; } = new List<FriendShip>();
        public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    }
}

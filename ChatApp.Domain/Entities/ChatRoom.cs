namespace ChatApp.Domain.Entities
{
    public class ChatRoom : BaseEntity    
    {
        public required string Name { get; set; }
        public required string CreatorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation 
        public AppUser? Creator { get; set; }
        public virtual ICollection<UserChatRoom>? UserChatRooms { get; set; } 
        public virtual ICollection<Message>? Messages { get; set; } 

    }
}

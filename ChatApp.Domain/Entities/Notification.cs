namespace ChatApp.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string? UserId { get; set; } 
        public int ChatRoomId { get; set; }
        public string? MessageContent { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public Group? ChatRoom { get; set; }
        public Message? Message { get; set; }
    }
}

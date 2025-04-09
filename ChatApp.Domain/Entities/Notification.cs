namespace ChatApp.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public required string SenderId { get; set; }
        public string? RecipientId { get; set; } 
        public int? GroupId { get; set; }
        public string? MessageContent { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties
        public AppUser? User { get; set; }
        public Group? ChatRoom { get; set; }
        public Message? Message { get; set; }
    }
}

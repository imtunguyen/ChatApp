namespace ChatApp.Domain.Entities
{
    public class ChatRoom : BaseEntity    
    {
        public required string Name { get; set; } 
        public string? CreatedById { get; set; } 
        public DateTime CreatedAt { get; set; }

        // Navigation 
        public ICollection<UserChatRoom>? UserChatRooms { get; set; } 
        public ICollection<Message>? Messages { get; set; } 
    }
}

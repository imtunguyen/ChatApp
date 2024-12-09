using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities
{
    public class Message : BaseEntity
    {
        public required string SenderId { get; set; }     
        public string? RecipientId { get; set; }
        public int? ChatRoomId { get; set; }
        public required string Content { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsRead { get; set; }      
        public bool IsDeleted { get; set; } = false;

        public AppUser? Sender { get; set; }
        public ChatRoom? ChatRoom { get; set; }
        public ICollection<MessageFile> Files { get; set; } = new List<MessageFile>();

        public void MarkAsRead()
        {
            Status = MessageStatus.Read;
            ReadAt = DateTimeOffset.UtcNow;
            IsRead = true;
        }

   
    }
}

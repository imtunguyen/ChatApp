using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities
{
    public class Message : BaseEntity
    {
        public required string SenderId { get; set; }     
        public string? RecipientId { get; set; }   
        public required MessageContent Content { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReadAt { get; set; }
        public bool IsRead { get; set; }      

        public AppUser? Sender { get; set; }        
        public ICollection<MessageFile> Files { get; set; } = new List<MessageFile>();
    }
}

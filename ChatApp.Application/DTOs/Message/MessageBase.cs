using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageBase
    {
        public required string SenderId { get; set; }
        public string? RecipientId { get; set; }
        public int? ChatRoomId { get; set; }
        public string? Content { get; set; }
    }
}

using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageBase
    {
        public required string SenderId { get; set; }
        public string? RecipientId { get; set; }
        public MessageContent? Content { get; set; }
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
    }
}

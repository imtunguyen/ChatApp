using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageDto : MessageBase
    {
        public int Id { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType Type { get; set; }
        public List<MessageFileDto> Files { get; set; } = new List<MessageFileDto>();

    }
}

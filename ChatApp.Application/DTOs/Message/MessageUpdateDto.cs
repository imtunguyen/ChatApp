using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageUpdateDto : MessageBase
    {
        public int Id { get; set; }
        public MessageStatus Status { get; set; }
    }
}

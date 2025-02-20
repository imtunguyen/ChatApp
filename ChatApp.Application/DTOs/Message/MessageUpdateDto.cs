using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageUpdateDto 
    {
        public int Id { get; set; }
        public MessageStatus Status { get; set; }
        public string? Content { get; set; }
    }
}

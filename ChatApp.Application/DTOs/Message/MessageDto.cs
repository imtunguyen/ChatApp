namespace ChatApp.Application.DTOs.Message
{
    public class MessageDto : MessageBase
    {
        public int Id { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReadAt { get; set; }
        public List<MessageFileDto> Files { get; set; } = new List<MessageFileDto>();

    }
}

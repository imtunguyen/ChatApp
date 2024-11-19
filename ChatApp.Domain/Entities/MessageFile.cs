namespace ChatApp.Domain.Entities
{
    public class MessageFile : BaseEntity
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }

        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}

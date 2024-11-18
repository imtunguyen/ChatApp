namespace ChatApp.Domain.Entities
{
    public class MessageFile : BaseEntity
    {
        public required string FilePath { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}

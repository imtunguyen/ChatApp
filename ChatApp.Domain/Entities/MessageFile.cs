using ChatApp.Domain.Enums;
using Microsoft.VisualBasic.FileIO;

namespace ChatApp.Domain.Entities
{
    public class MessageFile : BaseEntity
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }
        public FileType FileType { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
    }
}

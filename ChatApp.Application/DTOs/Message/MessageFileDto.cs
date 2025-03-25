using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageFileDto
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string? FileName { get; set; }
        public FileType FileType { get; set; }
    }
}

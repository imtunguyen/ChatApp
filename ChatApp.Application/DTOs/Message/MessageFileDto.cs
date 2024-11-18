namespace ChatApp.Application.DTOs.Message
{
    public class MessageFileDto
    {
        public required string FilePath { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; }
    }
}

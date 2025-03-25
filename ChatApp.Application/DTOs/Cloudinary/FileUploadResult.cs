using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.Cloudinary
{
    public class FileUploadResult
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
        public string? Error { get; set; }
        public string? FileName { get; set; }
        public FileType FileType { get; set; }
        public long FileSize { get; set; }
    }
}

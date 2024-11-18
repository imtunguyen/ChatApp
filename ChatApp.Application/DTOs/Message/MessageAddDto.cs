using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.Message
{
    public class MessageAddDto : MessageBase
    {
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}

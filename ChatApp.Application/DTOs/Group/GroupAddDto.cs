using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.Group
{
    public class GroupAddDto : GroupBase
    {
        public List<string>? UserIds { get; set; }
        public IFormFile? AvatarUrl { get; set; }
    }
}

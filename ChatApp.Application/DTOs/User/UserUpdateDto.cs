
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.User
{
    public class UserUpdateDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? Gender { get; set; }
    }
}

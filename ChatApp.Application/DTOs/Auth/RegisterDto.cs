using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public GenderType Gender { get; set; }
        public IFormFile? ProfilePicture { get; set; }

    }
}

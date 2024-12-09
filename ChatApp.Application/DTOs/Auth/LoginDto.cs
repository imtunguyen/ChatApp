using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.DTOs.Auth
{
    public class LoginDto
    {
        public required string UserNameOrEmail { get; set; }
        public required string Password { get; set; }
    }
}

using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public required string Token { get; set; } 
        public required Password NewPassword { get; set; }
    }
}

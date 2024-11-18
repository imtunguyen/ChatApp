using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public required string FullName {  get; set; }
        public required Username UserName { get; set; }
        public  required EmailAddress Email { get; set; }
        public required Password Password { get; set; }
        public GenderType Gender { get; set; }

    }
}

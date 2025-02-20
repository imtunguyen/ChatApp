using ChatApp.Application.DTOs.User;

namespace ChatApp.Application.Abstracts.Services.Identity
{
    public static class Response
    {
        public class LoginRespone
        {
            public UserDto User { get; set; }
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime? RefreshTokenExpriedtime { get; set; }
        }
    }
}


namespace ChatApp.Application.DTOs.User
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Gender { get; set; }
        public string? Token { get; set; }
        public string? RoleId { get; set; }
        public DateTimeOffset? RemovedAt { get; set; }
        public bool IsRemoved { get; set; }
    }
}

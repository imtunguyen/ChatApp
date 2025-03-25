namespace ChatApp.Application.DTOs.UserGroup
{
    public class UserGroupBase
    {
        public required string UserId { get; set; }
        public int GroupId { get; set; }
        public string? RoleId { get; set; }
        public bool IsRemoved { get; set; }    

    }
}

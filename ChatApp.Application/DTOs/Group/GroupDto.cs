namespace ChatApp.Application.DTOs.Group
{
    public class GroupDto : GroupBase
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public GroupFileDto? File { get; set; }
    }
}

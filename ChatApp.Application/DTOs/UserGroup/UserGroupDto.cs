namespace ChatApp.Application.DTOs.UserGroup
{
    public class UserGroupDto : UserGroupBase
    {
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset? RemovedAt { get; set; }
    }
}

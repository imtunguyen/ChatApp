namespace ChatApp.Application.DTOs.UserChatRoom
{
    public class UserChatRoomDto : UserChatRoomBase
    {
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset RemovedAt { get; set; }
    }
}

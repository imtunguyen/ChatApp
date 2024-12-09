namespace ChatApp.Application.DTOs.ChatRoom
{
    public class ChatRoomDto : ChatRoomBase
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}

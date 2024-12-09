namespace ChatApp.Domain.Entities
{
    public class UserChatRoom
    {
        public required string UserId { get; set; }   
        public int ChatRoomId { get; set; }     
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset RemovedAt { get; set; }

        public AppUser? User { get; set; }    
        public ChatRoom? ChatRoom { get; set; }  
    }
}

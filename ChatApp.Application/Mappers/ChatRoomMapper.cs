using ChatApp.Application.DTOs.ChatRoom;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappers
{
    public class ChatRoomMapper
    {
        public static ChatRoom ChatRoomAddDtoToEntity(ChatRoomAddDto chatRoom)
        {
            return new ChatRoom
            {
                Name = chatRoom.Name,
                CreatorId = chatRoom.CreatorId,
                CreatedAt = DateTimeOffset.UtcNow,
            };
        }
        public static ChatRoom ChatRoomUpdateDtoToEntity(ChatRoomUpdateDto chatRoom)
        {
            return new ChatRoom
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name,
                CreatorId = chatRoom.CreatorId,
            };
        }
        public static ChatRoomDto EntityToChatRoomDto(ChatRoom chatRoom)
        {
            return new ChatRoomDto
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name,
                CreatorId = chatRoom.CreatorId,
                CreatedAt = chatRoom.CreatedAt,
            };
        }
    }
}

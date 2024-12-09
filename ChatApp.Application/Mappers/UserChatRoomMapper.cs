using ChatApp.Application.DTOs.UserChatRoom;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappers
{
    public class UserChatRoomMapper
    {
        public static UserChatRoom AddDtoToEntity(UserChatRoomAddDto userChatRoomAddDto)
        {
            return new UserChatRoom
            {
                UserId = userChatRoomAddDto.UserId,
                ChatRoomId = userChatRoomAddDto.ChatRoomId,
                JoinedAt = DateTimeOffset.UtcNow,

            };
        }
        public static UserChatRoom UpdateDtoToEntity(UserChatRoomUpdateDto userChatRoomUpdateDto)
        {
            return new UserChatRoom
            {
                UserId = userChatRoomUpdateDto.UserId,
                ChatRoomId = userChatRoomUpdateDto.ChatRoomId,
                RemovedAt = DateTimeOffset.UtcNow,

            };
        }
        public static UserChatRoomDto EntityToDto(UserChatRoom userChatRoom)
        {
            return new UserChatRoomDto
            {
                UserId = userChatRoom.UserId,
                ChatRoomId = userChatRoom.ChatRoomId,
                JoinedAt = userChatRoom.JoinedAt,
                RemovedAt = userChatRoom.RemovedAt,
            };
        }

    }
}

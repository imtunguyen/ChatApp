using ChatApp.Application.DTOs.ChatRoom;
using ChatApp.Application.DTOs.UserChatRoom;
using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IChatRoomService
    {
        Task<ChatRoomDto> CreateChatRoomAsync(ChatRoomAddDto chatRoom);
        Task<ChatRoomDto> UpdateChatRoomAsync(ChatRoomUpdateDto chatRoom);
        Task<IEnumerable<ChatRoomDto>> GetChatRoomsByUserAsync(string userId);
        Task<bool> DeleteChatRoomAsync(Expression<Func<ChatRoom, bool>> expression);
        Task<ChatRoomDto> GetChatRoomByIdAsync(int id);


        Task<UserChatRoomDto> AddUserToChatRoom(UserChatRoomAddDto userChatRoomAddDto);
        Task<UserChatRoomDto> UpdateUserChatRoom(UserChatRoomUpdateDto userChatRoomUpdateDto);
        Task<bool> DeleteUserChatRoom(string userId, int chatRoomId);
        Task<PagedList<UserChatRoomDto>> GetUsersInChatRoomAsync(UserParams userParams, int chatRoomId);
        Task<UserChatRoomDto> GetUserInChatRoom(Expression<Func<UserChatRoom, bool>> expression);

    }
}

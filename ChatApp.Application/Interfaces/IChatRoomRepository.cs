using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRoomRepository : IRepository<ChatRoom>
    {
        Task<IEnumerable<ChatRoom>> GetChatRoomsByUser(string userId);
        Task UpdateChatRoomAsync(ChatRoom chatRoom);
    }
}

using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {

        Task UpdateMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesByUser(string userId);
        Task<Message> GetMessageByIdAsync(int id);
        Task<PagedList<Message>> GetMessagesThreadAsync(MessageParams baseParams, string senderId, string recipientId);
        Task<PagedList<Message>> GetMessagesGroupAsync(MessageParams messageParams, int groupId);
        Task<Message?> GetLastMessageAsync(string senderId, string recipientId);
        Task<PagedList<Message>> GetAllAsync(MessageParams baseParams, bool tracked = false);
    }
}

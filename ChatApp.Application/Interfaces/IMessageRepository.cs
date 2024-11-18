using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByUser(string userId);
        Task<PagedList<Message>> GetAllAsync(MessageParams baseParams, bool tracked = false);
    }
}

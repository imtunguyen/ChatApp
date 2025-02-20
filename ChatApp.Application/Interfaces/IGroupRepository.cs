using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<IEnumerable<Group>> GetGroupsByUser(string userId);
        Task UpdateGroupAsync(Group Group);
    }
}

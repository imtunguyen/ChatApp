using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IUserGroupRepository :IRepository<UserGroup>
    {
        Task<IEnumerable<AppUser>> GetUsersInGroup(int GroupId);
    }
}

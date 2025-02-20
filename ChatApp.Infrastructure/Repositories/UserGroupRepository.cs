using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        private readonly ChatAppContext _context;
        public UserGroupRepository(ChatAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetUsersInGroup(int GroupId)
        {
            var users = await _context.UserGroups
                .Where(ucr => ucr.GroupId == GroupId)
                .Select(ucr => ucr.User)
                .ToListAsync();
            return users;
        }
    }
}

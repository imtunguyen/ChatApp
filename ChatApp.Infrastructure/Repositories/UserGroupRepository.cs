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

        public async Task<List<UserGroup>> GetUsersInGroupAsync(int groupId)
        {
            return await _context.UserGroups
                .Where(ug => ug.GroupId == groupId && ug.IsRemoved == false)
                .Include(ug => ug.User ) 
                .ToListAsync();
        }
       

    }
}

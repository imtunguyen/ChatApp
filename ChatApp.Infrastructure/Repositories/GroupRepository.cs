using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly ChatAppContext _context;
        public GroupRepository(ChatAppContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Group>> GetGroupsByUser(string userId)
        {
            var Groups = await _context.UserGroups
                .Include(uc => uc.Group)
                .Where(uc => uc.UserId == userId && uc.IsRemoved == false)
                .Select(uc => uc.Group!)
                .ToListAsync();
            return Groups;
           
        }

        public async Task UpdateGroupAsync(Group Group)
        {
            var GroupEntity = await _context.Groups.FindAsync(Group.Id);
            if (GroupEntity != null)
            {
                GroupEntity.Name = Group.Name;
                GroupEntity.IsDeleted = Group.IsDeleted;
                GroupEntity.UpdatedAt = DateTimeOffset.UtcNow;
            }
           
        }
    }
}

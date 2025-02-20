using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Infrastructure.DataAccess;

namespace ChatApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatAppContext _context;
        public IMessageRepository MessageRepository { get; private set; }
        public IGroupRepository GroupRepository { get; private set; }
        public IUserGroupRepository UserGroupRepository { get; private set; }
        public IFriendShipRepository FriendShipRepository { get; private set; }
        public UnitOfWork(ChatAppContext context) 
        {
            _context = context;
            MessageRepository = new MessageRepository(_context);
            GroupRepository = new GroupRepository(_context);
            UserGroupRepository = new UserGroupRepository(_context);
            FriendShipRepository = new FriendShipRepository(_context);
        }
        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

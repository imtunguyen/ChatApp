using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Infrastructure.DataAccess;

namespace ChatApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatAppContext _context;
        public IMessageRepository MessageRepository { get; private set; }
        public IChatRoomRepository ChatRoomRepository { get; private set; }
        public IUserChatRoomRepository UserChatRoomRepository { get; private set; }
        public UnitOfWork(ChatAppContext context) 
        {
            _context = context;
            MessageRepository = new MessageRepository(_context);
            ChatRoomRepository = new ChatRoomRepository(_context);
            UserChatRoomRepository = new UserChatRoomRepository(_context);
        }
        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

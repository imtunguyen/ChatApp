using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRoomRepository : Repository<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomRepository(ChatAppContext context) : base(context)
        {
        }
    }
}

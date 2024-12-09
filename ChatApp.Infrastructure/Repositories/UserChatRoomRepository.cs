using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;

namespace ChatApp.Infrastructure.Repositories
{
    public class UserChatRoomRepository : Repository<UserChatRoom>, IUserChatRoomRepository
    {
        public UserChatRoomRepository(ChatAppContext context) : base(context)
        {
        }
    }
}

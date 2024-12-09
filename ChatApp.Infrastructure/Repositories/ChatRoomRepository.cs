using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRoomRepository : Repository<ChatRoom>, IChatRoomRepository
    {
        private readonly ChatAppContext _context;
        public ChatRoomRepository(ChatAppContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ChatRoom>> GetChatRoomsByUser(string userId)
        {
            var chatRooms = await _context.UserChatRooms
                .Include(uc => uc.ChatRoom)
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ChatRoom!)
                .ToListAsync();
            return chatRooms;
           
        }

        public async Task UpdateChatRoomAsync(ChatRoom chatRoom)
        {
            var chatRoomEntity = await _context.ChatRooms.FindAsync(chatRoom.Id);
            if (chatRoomEntity != null)
            {
                chatRoomEntity.Name = chatRoom.Name;
                chatRoomEntity.IsDeleted = chatRoom.IsDeleted;
                chatRoomEntity.UpdatedAt = DateTimeOffset.UtcNow;
            }
           
        }
    }
}

using ChatApp.Application.Interfaces;
using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.Ultilities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly ChatAppContext _context;
        public MessageRepository(ChatAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedList<Message>> GetAllAsync(MessageParams baseParams, bool tracked = false)
        {
            var query = tracked 
                ? _context.Messages.Include(m => m.Files).AsQueryable() 
                : _context.Messages.Include(m => m.Files).AsNoTracking().AsQueryable();
            if(!string.IsNullOrEmpty(baseParams.Search))
            {
                query = query.Where(m => m.Content != null &&
                            m.Content.ToLower().Contains(baseParams.Search.ToLower()));
            }

            if(!string.IsNullOrEmpty(baseParams.OrderBy))
            {
                query = baseParams.OrderBy switch
                {
                    "id" => query.OrderBy(m => m.Id),
                    "id_desc" => query.OrderByDescending(m => m.Id),
                    "content" => query.OrderBy(m => m.Content),
                    "content_desc" => query.OrderByDescending(m => m.Content),
                    _ => query.OrderByDescending(g => g.Id)
                };
            }
            return await query.ApplyPaginationAsync(baseParams.PageNumber, baseParams.PageSize);
        }
        public async Task<Message> GetMessageByIdAsync(int id)
        {
            var message = await _context.Messages.Include(m => m.Files).FirstOrDefaultAsync(m => m.Id == id);
            return message;
        }
        public async Task<IEnumerable<Message>> GetMessagesByUser(string userId)
        {
            return await _context.Messages.Include(m => m.Files).Where(m => m.SenderId == userId).ToListAsync();
        }

        public async Task<Message?> GetLastMessageAsync(string senderId, string recipientId)
        {
            return await _context.Messages
                .Include(m => m.Files)
                .Where(m => (m.SenderId == senderId && m.RecipientId == recipientId)
                    || (m.SenderId == recipientId && m.RecipientId == senderId))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<Message>> GetMessagesThreadAsync(MessageParams messageParams ,string senderId, string recipientId)
        {
            var query = _context.Messages
                .Include(m => m.Files)
                .Where(m => (m.SenderId == senderId && m.RecipientId == recipientId)
                    || (m.SenderId == recipientId && m.RecipientId == senderId) && m.IsDeleted == false)
                .OrderByDescending(m => m.SentAt)
                .AsQueryable();
            if (!string.IsNullOrEmpty(messageParams.Search))
            {
                query = query.Where(c =>
                    c.Content.ToLower().Contains(messageParams.Search.ToLower()));
            }
            
            return await query.ApplyPaginationAsync(messageParams.PageNumber, messageParams.PageSize);                                      
        }
        public async Task<PagedList<Message>> GetMessagesGroupAsync(MessageParams messageParams, int groupId)
        {
            var query = _context.Messages
                .Include(m => m.Files)
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.SentAt)
                .AsQueryable(); 
            if (!string.IsNullOrEmpty(messageParams.Search))
            {
                query = query.Where(c =>
                    c.Content.ToLower().Contains(messageParams.Search.ToLower()));
            }
            return await query.ApplyPaginationAsync(messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            var messageToUpdate = await _context.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
            if (messageToUpdate != null)
            {
                messageToUpdate.Content = message.Content;
                messageToUpdate.IsDeleted = message.IsDeleted;
                message.Status = message.Status;
                messageToUpdate.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        
    }
}

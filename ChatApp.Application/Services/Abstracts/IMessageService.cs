using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IMessageService
    {
        Task<MessageDto> AddMessageAsync(MessageAddDto messageAddDto);
        Task<MessageDto> UpdateMessageAsync(MessageUpdateDto messageUpdateDto);
        Task<bool> DeleteMessageAsync(Expression<Func<Message, bool>> expression);
        Task<MessageDto?> GetLastMessageAsync(string senderId, string recipientId);
        Task<PagedList<MessageDto>> GetMessagesThreadAsync(MessageParams messageParams, string senderId, string recipientId);
        Task<MessageDto> GetMessageByIdAsync(int id);
        Task<PagedList<MessageDto>> GetMessagesGroupAsync(MessageParams messageParams, int groupId); 
        Task<PagedList<MessageDto>> GetAllAsync(MessageParams messageParams, bool tracked);

        Task<bool> MarkMessageAsReadAsync(int messageId);
        Task<int> GetUnreadMessagesCountAsync(string userId);
    }
}

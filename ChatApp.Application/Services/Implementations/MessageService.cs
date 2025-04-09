using ChatApp.Domain.Entities;
using System.Linq.Expressions;
using ChatApp.Domain.Exceptions;
using ChatApp.Application.Mappers;
using ChatApp.Application.Utilities;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Parameters;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Abstracts.Services;

namespace ChatApp.Application.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        public MessageService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        //Add Mesage
        public async Task<MessageDto> AddMessageAsync(MessageAddDto messageAddDto)
        {
            var message = MessageMapper.MessageAddDtoToEntity(messageAddDto);

            if (messageAddDto.Files?.Any() == true)
            {
                var uploadTasks = messageAddDto.Files.Select(file =>
                {
                    return _cloudinaryService.UploadAsync(file);
                });

                var uploadResults = await Task.WhenAll(uploadTasks);

                var messageFiles = uploadResults.Select(result => new MessageFile
                {
                    PublicId = result.PublicId,
                    Url = result.Url,
                    FileName = result.FileName,
                    FileSize = result.FileSize,
                    FileType = result.FileType
                }).ToList();

                message.Files.AddRange(messageFiles);
            }
            await _unitOfWork.MessageRepository.AddAsync(message);

         
            return await _unitOfWork.CompleteAsync()
                ? MessageMapper.EntityToMessageDto(message)
                : throw new BadRequestException("Lỗi khi tạo tin nhắn");
        }
        //Update Message

        public async Task<MessageDto> UpdateMessageAsync(MessageUpdateDto messageUpdateDto)
        {
            
            var message = await _unitOfWork.MessageRepository.GetAsync(m => m.Id == messageUpdateDto.Id);
            if (message == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            message.Content = messageUpdateDto.Content;
            message.UpdatedAt = DateTimeOffset.UtcNow;
            message.Status = messageUpdateDto.Status;

            _unitOfWork.MessageRepository.Update(message);
            return await _unitOfWork.CompleteAsync()
                ? MessageMapper.EntityToMessageDto(message)
                : throw new BadRequestException("Lỗi khi cập nhật tin nhắn");
        }

        //Delete Message

        public async Task<bool> DeleteMessageAsync(Expression<Func<Message, bool>> expression)
        {
            var message = await _unitOfWork.MessageRepository.GetAsync(expression);
            if(message == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            message.IsDeleted = true;
            await _unitOfWork.MessageRepository.UpdateMessageAsync(message);
            return await _unitOfWork.CompleteAsync();
        }

        //Get Methods

        public async Task<PagedList<MessageDto>> GetAllAsync(MessageParams messageParams, bool tracked)
        {
            var messages = await _unitOfWork.MessageRepository.GetAllAsync(messageParams, tracked);
            if(messages == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            return new PagedList<MessageDto>(messages.Select(MessageMapper.EntityToMessageDto), messages.TotalCount, messages.CurrentPage, messages.PageSize);
        }

        public async Task<MessageDto?> GetLastMessageAsync(string senderId, string recipientId)
        {
            var message = await _unitOfWork.MessageRepository.GetLastMessageAsync(senderId, recipientId);
            if (message == null) return null;
            return MessageMapper.EntityToMessageDto(message);
        }

        public async Task<PagedList<MessageDto>> GetMessagesThreadAsync(MessageParams messageParams, string senderId, string recipientId)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessagesThreadAsync(messageParams, senderId, recipientId);
            if (messages == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            return new PagedList<MessageDto>(messages.Select(MessageMapper.EntityToMessageDto), messages.TotalCount, messages.CurrentPage, messages.PageSize);
        }

        public async Task<MessageDto> GetMessageByIdAsync(int messageId)
        {
            var message = await _unitOfWork.MessageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            return MessageMapper.EntityToMessageDto(message);
        }

        public Task<int> GetUnreadMessagesCountAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            var message = await _unitOfWork.MessageRepository.GetUnreadMessageByIdAsync(messageId);
            if (message != null)
            {
                message.MarkAsRead();
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;
        }

        public async Task<PagedList<MessageDto>> GetMessagesGroupAsync(MessageParams messageParams, int groupId)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessagesGroupAsync(messageParams, groupId);
            if (messages == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
            return new PagedList<MessageDto>(messages.Select(MessageMapper.EntityToMessageDto), messages.TotalCount, messages.CurrentPage, messages.PageSize);
        }
    }
}

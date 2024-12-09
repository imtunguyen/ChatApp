using ChatApp.Domain.Entities;
using System.Linq.Expressions;
using ChatApp.Domain.Exceptions;
using ChatApp.Application.Mappers;
using ChatApp.Application.Utilities;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Parameters;
using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Services.Abstracts;

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
            if (messageAddDto.Files != null)
            {
                foreach (var file in messageAddDto.Files)
                {
                    var uploadResult = await _cloudinaryService.UploadFileAsync(file);
                    message.Files.Add(new MessageFile
                    {
                        Url = uploadResult.Url!.ToString(),
                        PublicId = uploadResult.PublicId
                    });
                }
            }
            await _unitOfWork.MessageRepository.AddAsync(message);
            return await _unitOfWork.CompleteAsync()
                ? MessageMapper.EntityToMessageDto(message)
                : throw new BadRequestException("Lỗi khi tạo tin nhắn");

        }
        //Update Message

        public async Task<MessageDto> UpdateMessageAsync(MessageUpdateDto messageUpdateDto)
        {
            var message = MessageMapper.MessageUpdateDtoToEntity(messageUpdateDto);
            message = await _unitOfWork.MessageRepository.GetAsync(m => m.Id == messageUpdateDto.Id);
            if (message == null)
            {
                throw new NotFoundException("Không tìm thấy tin nhắn");
            }
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

        public  Task<MessageDto> GetLastMessageAsync(int messageId)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<MessageDto>> GetMessagesByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUnreadMessagesCountAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            throw new NotImplementedException();
        }


    }
}

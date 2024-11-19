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
        private readonly IUnitOfWork _unit;
        private readonly ICloudinaryService _cloudinaryService;
        public MessageService(IUnitOfWork unit, ICloudinaryService cloudinaryService)
        {
            _unit = unit;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<MessageDto> AddMessageAsync(MessageAddDto messageAddDto)
        {
            var message = MessageMapper.MessageAddDtoToEntity(messageAddDto);
            if (messageAddDto.Files != null)
            {
                foreach(var file in messageAddDto.Files)
                {
                    var uploadResult = await _cloudinaryService.UploadFileAsync(file, messageAddDto.Type);
                    if (uploadResult.Error != null)
                    {
                        throw new BadRequestException("Lỗi khi thêm file");
                    }
                    var messageFile = new MessageFile
                    {
                        PublicId = uploadResult.PublicId,
                        Url = uploadResult.Url,
                    };
                    message.Files.Add(messageFile);

                }
            }
            await _unit.MessageRepository.AddAsync(message);
            return await _unit.CompleteAsync()
                ? MessageMapper.EntityToMessageDto(message)
                : throw new BadRequestException("Thêm tin nhắn thất bại");
        }
        public async Task<MessageDto> UpdateMessageAsync(MessageUpdateDto messageUpdateDto)
        {
            throw new NotImplementedException();
        }
        public async Task<MessageDto> DeleteMessageAsync(Expression<Func<Message, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MessageDto>> GetAllAsync(MessageParams messageParams, bool tracked)
        {
            var messages = await _unit.MessageRepository.GetAllAsync(messageParams, tracked);
            var messageDto = messages.Select(MessageMapper.EntityToMessageDto);
            return new PagedList<MessageDto>(messageDto, messages.TotalCount, messages.CurrentPage, messages.PageSize);
        }

        public async Task<MessageDto> GetLastMessageAsync(int messageId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageDto> GetMessageByIdAsync(int id)
        {
            var message = await _unit.MessageRepository.GetMessageByIdAsync(id);
            return message != null 
                ? MessageMapper.EntityToMessageDto(message) 
                : throw new NotFoundException("Tin nhắn không tồn tại");
        }
    }
}

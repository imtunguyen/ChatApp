using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using System.Linq.Expressions;

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

                }

            }
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
            throw new NotImplementedException();
        }

        public async Task<MessageDto> GetLastMessageAsync(int messageId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        
    }
}

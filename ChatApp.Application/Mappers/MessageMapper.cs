using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappers
{
    public class MessageMapper
    {
        public static Message MessageAddDtoToEntity(MessageAddDto messageAddDto)
        {
            return new Message
            {
                Content = messageAddDto.Content,
                SenderId = messageAddDto.SenderId,
                RecipientId = messageAddDto.RecipientId,
                Type = messageAddDto.Type,
                Status = messageAddDto.Status,
                SentAt = DateTime.UtcNow.AddHours(7),
            };
        }
        public static Message MessageUpdateDtoToEntity(MessageUpdateDto messageUpdateDto)
        {
            return new Message
            {
                Id = messageUpdateDto.Id,
                Content = messageUpdateDto.Content,
                SenderId = messageUpdateDto.SenderId,
                RecipientId = messageUpdateDto.RecipientId,
                Type = messageUpdateDto.Type,
                Status = messageUpdateDto.Status,
                SentAt = DateTime.UtcNow.AddHours(7),
            };
        }
        public static MessageDto EntityToMessageDto(Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                RecipientId = message.RecipientId,
                Type = message.Type,
                Status = message.Status,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt,
            };
        }

    }
}

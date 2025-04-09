using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Mappers
{
    public class MessageMapper
    {
        public static Message MessageAddDtoToEntity(MessageAddDto messageAddDto)
        {
            var message = new Message
            {
                SenderId = messageAddDto.SenderId,
                RecipientId = messageAddDto.RecipientId,
                GroupId = messageAddDto.GroupId,
                Content = messageAddDto.Content ?? string.Empty,
                Files = new List<MessageFile>(),
                Status = MessageStatus.Sent,
                SentAt = DateTimeOffset.UtcNow,
            };

            message.SetMessageType();
            return message;
        }
        public static Message MessageUpdateDtoToEntity(MessageUpdateDto messageUpdateDto)
        {
            return new Message
            {
                Id = messageUpdateDto.Id,
                SenderId = messageUpdateDto.SenderId,
                GroupId = messageUpdateDto.GroupId,
                Content = messageUpdateDto.Content ?? string.Empty,
                Status = messageUpdateDto.Status,
                SentAt = DateTimeOffset.UtcNow,
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
                GroupId = message.GroupId,
                Status = message.Status,
                Type = message.Type,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt,
                IsRead = message.IsRead,
                IsDeleted = message.IsDeleted,
                Files = message.Files.Select(MessageFileToMessageFileDto).ToList(),
            };
        }
        public static MessageFileDto MessageFileToMessageFileDto(MessageFile messageFile)
        {
            return new MessageFileDto
            {
                Id = messageFile.Id,
                Url = messageFile.Url ?? string.Empty,
                FileType = messageFile.FileType                
            };

        }
       
    }
}

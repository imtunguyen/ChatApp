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
            return new Message
            {
                Content = messageAddDto.Content ?? string.Empty,
                SenderId = messageAddDto.SenderId,
                RecipientId = messageAddDto.RecipientId,
                GroupId = messageAddDto.GroupId,
                Type = GetMessageType(messageAddDto.Files),
                Status = MessageStatus.Sent, 
                SentAt = DateTime.UtcNow.AddHours(7),
            };
        }
        public static Message MessageUpdateDtoToEntity(MessageUpdateDto messageUpdateDto)
        {
            return new Message
            {
                Id = messageUpdateDto.Id,
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
                Files = message.Files.Select(MessageFileToMessageFileDto).ToList(),
            };
        }
        public static MessageFileDto MessageFileToMessageFileDto(MessageFile messageFile)
        {
            return new MessageFileDto
            {
                Id = messageFile.Id,
                Url = messageFile.Url ?? string.Empty,
            };

        }
        public static MessageType GetMessageType(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return MessageType.Text;
            }
            foreach(var file in files)
            {
                if (file.ContentType.Contains("image"))
                {
                    return MessageType.Image;
                }
                if (file.ContentType.Contains("video"))
                {
                    return MessageType.Video;
                }
                if (file.ContentType.Contains("audio"))
                {
                    return MessageType.Audio;
                }
                if (file.ContentType.Contains("application"))
                {
                    return MessageType.File;
                }
            }
            return MessageType.Text;
        }
    }
}

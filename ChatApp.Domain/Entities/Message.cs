using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities
{
    public class Message : BaseEntity
    {
        public required string SenderId { get; set; }
        public string? RecipientId { get; set; }
        public int? GroupId { get; set; }

        public MessageType Type { get; private set; }
        public MessageStatus Status { get; set; }
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ReadAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsRead { get; private set; } = false;
        public bool IsDeleted { get; set; } = false;

        public string? Content { get; set; }
        public List<MessageFile> Files { get; set; } = new List<MessageFile>();

        public AppUser? Sender { get; set; }
        public Group? Group { get; set; }

        public Message()
        {
            SetMessageType();
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                Status = MessageStatus.Read;
                ReadAt = DateTimeOffset.UtcNow;
                IsRead = true;
            }
        }

        public void SetMessageType()
        {
            if (!string.IsNullOrWhiteSpace(Content) && Files.Any())
            {
                Type = MessageType.Mixed;
            }
            else if (!string.IsNullOrWhiteSpace(Content))
            {
                Type = MessageType.Text;
            }
            else if (Files.Any())
            {
                Type = GetMessageFileType();
            }
            else
            {
                Type = MessageType.Unknown;
            }
        }

        private MessageType GetMessageFileType()
        {
            return Files.FirstOrDefault()?.FileType switch
            {
                FileType.Image => MessageType.Image,
                FileType.Video => MessageType.Video,
                FileType.Audio => MessageType.Audio,
                _ => MessageType.File
            };
        }
    }
}

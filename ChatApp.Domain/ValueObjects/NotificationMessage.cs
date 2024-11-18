using ChatApp.Domain.Exceptions;

namespace ChatApp.Domain.ValueObjects
{
    public class NotificationMessage
    {
        public string Value { get; private set; }

        public NotificationMessage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException("Notification message cannot be empty.");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is NotificationMessage notification)
                return Value == notification.Value;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

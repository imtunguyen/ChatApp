using ChatApp.Domain.Exceptions;

namespace ChatApp.Domain.ValueObjects
{
    public class MessageContent
    {
        public string Value { get; private set; }

        public MessageContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 500) 
                throw new BadRequestException("Message content is invalid.");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageContent content)
                return Value == content.Value;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

using ChatApp.Domain.Exceptions;

namespace ChatApp.Domain.ValueObjects
{
    public class Timestamp
    {
        public DateTime Value { get; private set; }

        public Timestamp(DateTime value)
        {
            if (value == default)
                throw new BadRequestException("Invalid timestamp.");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Timestamp timestamp)
                return Value == timestamp.Value;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

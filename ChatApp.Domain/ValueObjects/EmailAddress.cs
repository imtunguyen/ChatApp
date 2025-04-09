using ChatApp.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ChatApp.Domain.ValueObjects
{
    public class EmailAddress
    {
        public string Value { get; private set; }

        public EmailAddress(string value)
        {
            if(string.IsNullOrEmpty(value) || !IsValidEmail(value))
                throw new BadRequestException("Vui lòng nhập email hợp lệ");
            
            Value = value;
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
        public override bool Equals(object obj)
        {
            if(obj is EmailAddress email)
                return Value == email.Value;
            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

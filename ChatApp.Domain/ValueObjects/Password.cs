using ChatApp.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ChatApp.Domain.ValueObjects
{
    public class Password
    {
        public string Value { get; private set; }
        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidPassword(value))
                throw new BadRequestException(
                    "Mật khẩu phải dài ít nhất 6 ký tự và bao gồm chữ hoa, chữ thường, một số và một ký tự đặc biệt.");

            Value = value;
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 6) return false;
            
            bool hasUpperCase = Regex.IsMatch(password, "[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, "[a-z]");
            bool hasDigit = Regex.IsMatch(password, "[0-9]");
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]");
            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
        public override bool Equals(object obj)
        {
            if (obj is Password password)
                return Value == password.Value;

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

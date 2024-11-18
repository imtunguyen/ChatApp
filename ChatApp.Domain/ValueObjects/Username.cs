﻿
using ChatApp.Domain.Exceptions;

namespace ChatApp.Domain.ValueObjects
{
    public class Username
    {
        public string Value { get; private set; }

        public Username(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 3 || value.Length > 20)
                throw new BadRequestException("Invalid username.");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Username username)
                return Value == username.Value;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

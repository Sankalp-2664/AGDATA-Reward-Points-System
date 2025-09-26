using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public sealed class Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.");

            if (!value.Contains("@"))
                throw new ArgumentException("Invalid email format.");

            // Restrict to AGDATA employees
            if (!value.EndsWith("@agdata.com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only AGDATA employees can register.");

            Value = value;
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) =>
            obj is Email email && Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

}

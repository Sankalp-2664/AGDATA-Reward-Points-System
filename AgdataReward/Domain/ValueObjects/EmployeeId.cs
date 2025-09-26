using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public sealed class EmployeeId
    {
        public string Value { get; }

        public EmployeeId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Employee ID cannot be empty.");

            if (value.Length < 3)
                throw new ArgumentException("Employee ID must be at least 3 characters long.");

            Value = value;
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) =>
            obj is EmployeeId empId && Value.Equals(empId.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

}

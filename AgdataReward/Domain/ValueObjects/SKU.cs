using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public sealed class SKU
    {
        public string Value { get; }

        public SKU(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU cannot be empty.");

            if (value.Length < 4)
                throw new ArgumentException("SKU must be at least 4 characters long.");

            Value = value.ToUpperInvariant();
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) =>
            obj is SKU sku && Value.Equals(sku.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

}

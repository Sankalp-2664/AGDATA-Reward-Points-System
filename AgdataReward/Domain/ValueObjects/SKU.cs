using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class SKU : IEquatable<SKU>
{
    public string Value { get; }

    public SKU(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU cannot be empty or whitespace.", nameof(value));

        if (value.Length < 4)
            throw new ArgumentException("SKU must be at least 4 characters long.", nameof(value));

        // Optional: enforce only letters, numbers, hyphens
        if (!Regex.IsMatch(value, @"^[A-Za-z0-9\-]+$"))
            throw new ArgumentException("SKU must only contain letters, numbers, or hyphens.", nameof(value));

        Value = value.ToUpperInvariant(); // normalize
    }

    // Equality
    public override bool Equals(object? obj) => Equals(obj as SKU);

    public bool Equals(SKU? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    // Operators
    public static bool operator ==(SKU? left, SKU? right) => Equals(left, right);
    public static bool operator !=(SKU? left, SKU? right) => !Equals(left, right);

    // Conversions
    public static implicit operator string(SKU sku) => sku.Value;
    public static explicit operator SKU(string value) => new SKU(value);
}

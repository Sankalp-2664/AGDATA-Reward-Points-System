namespace Domain.Exceptions;

public sealed class InvalidRedemptionException : DomainException
{
    public InvalidRedemptionException(string reason)
        : base($"Invalid redemption: {reason}") { }
}

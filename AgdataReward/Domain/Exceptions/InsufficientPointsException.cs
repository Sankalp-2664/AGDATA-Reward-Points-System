namespace Domain.Exceptions;

public sealed class InsufficientPointsException : DomainException
{
    public int CurrentBalance { get; }
    public int Attempted { get; }

    public InsufficientPointsException(int available, int required)
        : base($"Insufficient points. Available: {available}, Required: {required}.")
    {
        CurrentBalance = available;
        Attempted = required;
    }
}

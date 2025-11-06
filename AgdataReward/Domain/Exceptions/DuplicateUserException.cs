namespace Domain.Exceptions;

public sealed class DuplicateUserException : DomainException
{
    public DuplicateUserException(string emailOrId)
        : base($"A user with identifier '{emailOrId}' already exists.") { }
}

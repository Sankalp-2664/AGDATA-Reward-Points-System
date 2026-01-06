using Domain.Entities.User;

namespace Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Registers a new user with the specified role.
    /// </summary>
    Task<UserProfile> RegisterUserAsync(string employeeId, string email, string firstName, string lastName, string roleName, string password);

    /// <summary>
    /// Gets a user profile by email.
    /// </summary>
    Task<UserProfile?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Gets the user's reward account by user ID.
    /// </summary>
    Task<UserAccount?> GetUserAccountAsync(Guid userId);
    Task<UserProfile?> GetUserByIdAsync(Guid userId);
}

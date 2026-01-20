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

    /// <summary>
    /// Gets all users.
    /// </summary>
    Task<List<UserProfile>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user's profile.
    /// </summary>
    Task<UserProfile?> UpdateUserAsync(Guid userId, string firstName, string lastName, string email, string roleName, string? accountStatus = null);

}

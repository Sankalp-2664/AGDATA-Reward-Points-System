using Domain.Entities.User;
using Domain.ValueObjects;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Using value object Email (for domain-accurate calls)
    Task<UserProfile?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // Convenience overload for application/auth layer (string from DTO)
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByEmployeeIdAsync(
        EmployeeId employeeId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> FindByEmailOrEmployeeIdAsync(
        Email email,
        EmployeeId employeeId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<UserProfile>> ListAsync(CancellationToken cancellationToken = default);

    Task AddAsync(UserProfile user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile user, CancellationToken cancellationToken = default);
}

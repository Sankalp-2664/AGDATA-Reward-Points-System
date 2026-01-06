using Application.Interfaces;
using Domain.Entities.User;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserProfile> _users = new();

    public Task<UserProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<UserProfile?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = _users.FirstOrDefault(u =>
            u.Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = _users.FirstOrDefault(u =>
            u.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<UserProfile?> GetByEmployeeIdAsync(
        EmployeeId employeeId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = _users.FirstOrDefault(u =>
            u.EmployeeId.Value.Equals(employeeId.Value, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<UserProfile?> FindByEmailOrEmployeeIdAsync(
        Email email,
        EmployeeId employeeId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = _users.FirstOrDefault(u =>
            u.Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase) ||
            u.EmployeeId.Value.Equals(employeeId.Value, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<IEnumerable<UserProfile>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Return a copy to prevent external mutation
        return Task.FromResult<IEnumerable<UserProfile>>(_users.ToList());
    }

    public Task AddAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var index = _users.FindIndex(u => u.Id == user.Id);

        if (index >= 0)
        {
            _users[index] = user;
        }

        return Task.CompletedTask;
    }
}

using Application.Interfaces;
using Domain.Entities.User;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryRoleRepository : IRoleRepository
{
    private readonly List<Role> _roles = [];

    public Task<Role?> GetByIdAsync(Guid id)
        => Task.FromResult(_roles.FirstOrDefault(r => r.Id == id));

    public Task<Role?> GetByNameAsync(string name)
        => Task.FromResult(_roles.FirstOrDefault(
            r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

    public Task<bool> ExistsAsync(string name)
        => Task.FromResult(_roles.Any(
            r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Role role)
    {
        _roles.Add(role);
        return Task.CompletedTask;
    }
}

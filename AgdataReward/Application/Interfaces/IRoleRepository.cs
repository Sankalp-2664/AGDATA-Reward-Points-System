namespace Application.Interfaces;

using Domain.Entities.User;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<bool> ExistsAsync(string name);
}

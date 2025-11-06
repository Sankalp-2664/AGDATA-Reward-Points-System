using Domain.Entities.User;
using Domain.ValueObjects;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<UserProfile?> GetByEmailAsync(Email email);
    Task<UserProfile?> GetByEmployeeIdAsync(EmployeeId employeeId);
    Task<UserProfile?> FindByEmailOrEmployeeIdAsync(Email email, EmployeeId employeeId);

    Task<IEnumerable<UserProfile>> ListAsync();

    Task AddAsync(UserProfile user);
    Task UpdateAsync(UserProfile user);
}

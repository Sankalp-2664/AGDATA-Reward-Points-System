using Domain.Entities.User;

namespace Application.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserAccount account);
    Task UpdateAsync(UserAccount account);
}

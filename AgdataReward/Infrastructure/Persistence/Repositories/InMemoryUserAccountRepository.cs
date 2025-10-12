using Application.Interfaces;
using Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryUserAccountRepository : IUserAccountRepository
    {
        private readonly Dictionary<Guid, UserAccount> _accounts = new();

        public virtual Task<UserAccount?> GetByUserIdAsync(Guid userId)
        {
            _accounts.TryGetValue(userId, out var account);
            return Task.FromResult(account);
        }

        public Task UpdateAsync(UserAccount account)
        {
            _accounts[account.UserId] = account;
            return Task.CompletedTask;
        }

        public Task AddAsync(UserAccount account)
        {
            _accounts[account.UserId] = account;
            return Task.CompletedTask;
        }
    }
}

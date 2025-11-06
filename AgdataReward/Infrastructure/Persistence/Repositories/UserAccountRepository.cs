using Application.Interfaces;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly RewardDbContext _context;

        public UserAccountRepository(RewardDbContext context)
        {
            _context = context;
        }

        public async Task<UserAccount?> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserAccounts
                .Include(a => a.Transactions)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task UpdateAsync(UserAccount account)
        {
            _context.UserAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(UserAccount account)
        {
            await _context.UserAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }
    }
}

using Application.Interfaces;
using Domain.Entities.User;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RewardDbContext _context;

        public UserRepository(RewardDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByIdAsync(Guid id)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserProfile?> GetByEmailAsync(Email email)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Email.Value.ToLower() == email.Value.ToLower());
        }

        public async Task<UserProfile?> GetByEmployeeIdAsync(EmployeeId employeeId)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.EmployeeId.Value.ToLower() == employeeId.Value.ToLower());
        }

        public async Task<UserProfile?> FindByEmailOrEmployeeIdAsync(Email email, EmployeeId employeeId)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u =>
                    u.Email.Value.ToLower() == email.Value.ToLower() ||
                    u.EmployeeId.Value.ToLower() == employeeId.Value.ToLower());
        }

        public async Task<IEnumerable<UserProfile>> ListAsync()
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .ToListAsync();
        }

        public async Task AddAsync(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile user)
        {
            _context.UserProfiles.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

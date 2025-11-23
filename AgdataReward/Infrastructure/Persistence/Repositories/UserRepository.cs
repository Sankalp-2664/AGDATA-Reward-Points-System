using Application.Interfaces;
using Domain.Entities.User;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

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
                .FirstOrDefaultAsync(u => u.Email == email);  
        }

        public async Task<UserProfile?> GetByEmployeeIdAsync(EmployeeId employeeId)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);  
        }

        public async Task<UserProfile?> FindByEmailOrEmployeeIdAsync(Email email, EmployeeId employeeId)
        {
            return await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u =>
                    u.Email == email ||                 
                    u.EmployeeId == employeeId);           
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

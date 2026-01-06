using Application.Interfaces;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RewardDbContext _context;

    public RoleRepository(RewardDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Roles
            .AnyAsync(r => r.Name == name);
    }
}

using Application.Interfaces;
using Domain.Entities.User;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(RewardDbContext context) : IUserRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<UserProfile?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    // For login convenience
    public async Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var emailVo = new Email(email);

        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Email == emailVo, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmployeeIdAsync(
        EmployeeId employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken);
    }

    public async Task<UserProfile?> FindByEmailOrEmployeeIdAsync(
        Email email,
        EmployeeId employeeId,
        CancellationToken cancellationToken = default)
    {
        // This one stays FirstOrDefault because BOTH fields may exist separately
        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.Email == email || u.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(u => u.Account)
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
    {
        await _context.UserProfiles.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        UserProfile user,
        CancellationToken cancellationToken = default)
    {
        _context.UserProfiles.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

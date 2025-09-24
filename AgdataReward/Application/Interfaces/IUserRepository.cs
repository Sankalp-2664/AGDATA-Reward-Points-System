using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<UserProfile?> GetByEmailAsync(string email);
    Task<UserProfile?> GetByEmployeeIdAsync(string employeeId);
    Task AddAsync(UserProfile user);
    Task<IEnumerable<UserProfile>> ListAsync();
}

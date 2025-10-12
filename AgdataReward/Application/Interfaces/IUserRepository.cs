using Domain.Entities.User;
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
    Task<UserProfile?> FindByEmailOrEmployeeIdAsync(string email, string employeeId);


    Task<IEnumerable<UserProfile>> ListAsync();


    Task AddAsync(UserProfile user);
    Task UpdateAsync(UserProfile user);
}

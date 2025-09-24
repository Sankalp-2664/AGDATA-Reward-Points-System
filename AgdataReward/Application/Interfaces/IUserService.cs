using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfile> RegisterUserAsync(string employeeId, string email, string firstName, string lastName);
        Task<UserProfile?> GetUserByEmailAsync(string email);
        Task<UserAccount?> GetUserAccountAsync(Guid userId);
    }

}

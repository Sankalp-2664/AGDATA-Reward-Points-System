using Domain.Entities.User;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user with the specified role.
        /// </summary>
        Task<UserProfile> RegisterUserAsync(string employeeId, string email, string firstName, string lastName, UserRole role);

        /// <summary>
        /// Gets a user profile by email.
        /// </summary>
        Task<UserProfile?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets the user's reward account by user ID.
        /// </summary>
        Task<UserAccount?> GetUserAccountAsync(Guid userId);
    }

}

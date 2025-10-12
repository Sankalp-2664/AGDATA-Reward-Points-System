using Application.Interfaces;
using Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<UserProfile> _users = new();

        public Task<UserProfile?> GetByIdAsync(Guid id)
            => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

        public Task<UserProfile?> GetByEmailAsync(string email)
            => Task.FromResult(_users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

        public Task<UserProfile?> GetByEmployeeIdAsync(string employeeId)
            => Task.FromResult(_users.FirstOrDefault(u => u.EmployeeId.Equals(employeeId, StringComparison.OrdinalIgnoreCase)));

        public Task<UserProfile?> FindByEmailOrEmployeeIdAsync(string email, string employeeId)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                u.EmployeeId.Equals(employeeId, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(user);
        }

        public Task AddAsync(UserProfile user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserProfile>> ListAsync()
            => Task.FromResult<IEnumerable<UserProfile>>(_users);

        public Task UpdateAsync(UserProfile user)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
            }

            return Task.CompletedTask;
        }
    }
}

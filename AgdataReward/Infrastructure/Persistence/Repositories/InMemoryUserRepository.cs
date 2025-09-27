using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task AddAsync(UserProfile user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserProfile>> ListAsync()
            => Task.FromResult<IEnumerable<UserProfile>>(_users);
    }
}

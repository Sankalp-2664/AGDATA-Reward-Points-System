using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly List<UserProfile> _users = new();
        private int _nextId = 1; // simple counter for UserId

        public UserProfile AddUser(UserProfile user)
        {
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("User with this email already exists.");

            //generating new ids
            var newUser = new UserProfile(
                _nextId++,
                user.FirstName,
                user.LastName,
                user.Email,
                user.UserType
            );

            _users.Add(newUser);
            return newUser;
        }

        public UserProfile? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.UserId == id);
        }

        public IEnumerable<UserProfile> GetAllUsers()
        {
            return _users;
        }

        public UserProfile UpdateUser(UserProfile user)
        {
            var existing = _users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existing == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // check for duplicate email if it changed
            if (_users.Any(u => u.UserId != user.UserId &&
                                u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Another user with this email already exists.");

            _users.Remove(existing);
            _users.Add(user);
            return user;
        }
    }
}

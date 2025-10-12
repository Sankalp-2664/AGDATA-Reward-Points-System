using Application.Interfaces;
using Domain.Exceptions;
using Domain.ValueObjects;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.User;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountRepository _accountRepository;

        public UserService(IUserRepository userRepository, IUserAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task<UserProfile> RegisterUserAsync(string employeeId, string email, string firstName, string lastName, UserRole role)
        {
            var employee = new EmployeeId(employeeId);
            var userEmail = new Email(email);

            var existingUser = await _userRepository.FindByEmailOrEmployeeIdAsync(userEmail.Value, employee.Value);
            if (existingUser != null)
            {
                if (existingUser.Email == userEmail.Value)
                    throw new DuplicateUserException($"Email '{email}' is already registered.");

                if (existingUser.EmployeeId == employee.Value)
                    throw new DuplicateUserException($"Employee ID '{employeeId}' is already registered.");
            }

            if (!Enum.IsDefined(typeof(UserRole), role))
                throw new ArgumentException($"Invalid role: {role}", nameof(role));


            var profile = new UserProfile(
                Guid.NewGuid(),
                employee.Value,
                userEmail.Value,
                firstName,
                lastName,
                role
            );

            await _userRepository.AddAsync(profile);

            var account = new UserAccount(profile.Id);
            await _accountRepository.UpdateAsync(account);

            return profile;
        }

        public async Task<UserProfile?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<UserAccount?> GetUserAccountAsync(Guid userId)
        {
            return await _accountRepository.GetByUserIdAsync(userId);
        }
    }
}

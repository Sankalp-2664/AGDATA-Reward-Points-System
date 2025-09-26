using Application.Interfaces;
using Domain.Exceptions;
using Domain.ValueObjects;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<UserProfile> RegisterUserAsync(string employeeId, string email, string firstName, string lastName)
        {
            // Prevent duplicates
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new DuplicateUserException(email);

            existingUser = await _userRepository.GetByEmployeeIdAsync(employeeId);
            if (existingUser != null)
                throw new DuplicateUserException(employeeId);

            // Create new profile with ValueObjects
            var profile = new UserProfile(Guid.NewGuid(), new EmployeeId(employeeId).Value, new Email(email).Value, firstName, lastName);
            await _userRepository.AddAsync(profile);

            // Create account with 0 points
            var account = new UserAccount(profile.Id);
            await _accountRepository.UpdateAsync(account);

            return profile;
        }

        public async Task<UserProfile?> GetUserByEmailAsync(string email)
            => await _userRepository.GetByEmailAsync(email);

        public async Task<UserAccount?> GetUserAccountAsync(Guid userId)
            => await _accountRepository.GetByUserIdAsync(userId);
    }
}

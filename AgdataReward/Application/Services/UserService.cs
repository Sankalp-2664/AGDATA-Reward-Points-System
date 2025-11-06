using Application.Interfaces;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services;

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

        var existingUser = await _userRepository.FindByEmailOrEmployeeIdAsync(userEmail, employee);
        if (existingUser != null)
        {
            if (existingUser.Email.Value == userEmail.Value)
                throw new DuplicateUserException($"Email '{email}' is already registered.");

            if (existingUser.EmployeeId.Value == employee.Value)
                throw new DuplicateUserException($"Employee ID '{employeeId}' is already registered.");
        }

        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException($"Invalid role: {role}", nameof(role));


        var profile = new UserProfile(
            Guid.NewGuid(),
            employee,
            userEmail,
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
        var userEmail = new Email(email);
        return await _userRepository.GetByEmailAsync(userEmail);
    }

    public async Task<UserAccount?> GetUserAccountAsync(Guid userId)
    {
        return await _accountRepository.GetByUserIdAsync(userId);
    }
}

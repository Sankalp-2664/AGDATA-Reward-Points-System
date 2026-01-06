using Application.Interfaces;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUserAccountRepository accountRepository,
    IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserAccountRepository _accountRepository = accountRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<UserProfile> RegisterUserAsync(
        string employeeId,
        string email,
        string firstName,
        string lastName,
        UserRole role,
        string password)
    {
        // 1) Basic password check (you can make this stricter later)
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        // 2) Convert to value objects (validation inside them)
        var employee = new EmployeeId(employeeId);
        var userEmail = new Email(email);

        // 3) Check duplicates in ONE call
        var existingUser = await _userRepository.FindByEmailOrEmployeeIdAsync(userEmail, employee);
        if (existingUser != null)
        {
            if (existingUser.Email.Value == userEmail.Value)
                throw new DuplicateUserException($"Email '{email}' is already registered.");

            if (existingUser.EmployeeId.Value == employee.Value)
                throw new DuplicateUserException($"Employee ID '{employeeId}' is already registered.");
        }

        // 4) Validate role
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException($"Invalid role: {role}", nameof(role));

        // 5) Create aggregate root – Id is generated INSIDE the entity
        var profile = new UserProfile(
            employee,
            userEmail,
            firstName,
            lastName,
            role
        );

        // 6) Create account with credentials
        var account = new UserAccount(profile.Id);

        var result = _passwordHasher.Hash(password);
        var hash = result.Hash;
        var salt = result.Salt;

        account.SetCredentials(hash, salt);

        // 7) Attach to aggregate
        profile.AttachAccount(account);

        // 8) Persist profile + account
        // Depending on your repo implementations, you can either:
        // a) Save via profile only (if cascade is configured), or
        // b) Save both (current pattern).
        await _userRepository.AddAsync(profile);
        await _accountRepository.AddAsync(account);

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

    public async Task<UserProfile?> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return await _userRepository.GetByIdAsync(userId);
    }
}

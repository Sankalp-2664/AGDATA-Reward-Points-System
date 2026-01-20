using Application.Interfaces;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUserAccountRepository accountRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserAccountRepository _accountRepository = accountRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<UserProfile> RegisterUserAsync(
        string employeeId,
        string email,
        string firstName,
        string lastName,
        string role,
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
        var roleEntity = await _roleRepository.GetByNameAsync(role);
        if (roleEntity == null)
            throw new ArgumentException($"Invalid role: {role}", nameof(role));

        // 5) Create aggregate root – Id is generated INSIDE the entity
        var profile = new UserProfile(
            employee,
            userEmail,
            firstName,
            lastName
        );

        // ✅ Assign role via aggregate root (DDD rule)
        profile.AssignRole(roleEntity);

        // 6) Create account with credentials
        var account = new UserAccount(profile.Id);

        var result = _passwordHasher.Hash(password);
        var hash = result.Hash;
        var salt = result.Salt;

        account.SetCredentials(hash, salt);

        // 7) Attach to aggregate
        profile.AttachAccount(account);

        // 8) Persist profile + account in a single transaction
        // Since UserAccount is attached to UserProfile, we only need to add the profile
        // EF Core will handle the cascade insert of the account
        await _userRepository.AddAsync(profile);

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

    public async Task<List<UserProfile>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        return users.ToList();
    }

    public async Task<UserProfile?> UpdateUserAsync(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string roleName,
        string? accountStatus = null)
    {
        // Get existing user with all navigation properties
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        // Update basic info
        user.UpdateName(firstName.Trim(), lastName.Trim());

        // Update email if changed
        var newEmail = new Email(email);
        if (user.Email.Value != newEmail.Value)
        {
            // Check if new email is already taken
            var existingUser = await _userRepository.GetByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != userId)
                throw new DuplicateUserException($"Email '{email}' is already in use.");
            
            user.UpdateEmail(newEmail);
        }

        // Update role only if it has changed
        var currentRoleName = user.Roles.FirstOrDefault()?.Role?.Name;
        if (currentRoleName != roleName)
        {
            var newRole = await _roleRepository.GetByNameAsync(roleName);
            if (newRole == null)
                throw new ArgumentException($"Invalid role: {roleName}", nameof(roleName));

            // Remove existing roles
            var existingRoles = user.Roles.ToList();
            foreach (var userRole in existingRoles)
            {
                // Need to load the Role entity if not loaded
                if (userRole.Role == null)
                {
                    var roleEntity = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (roleEntity != null)
                        user.RemoveRole(roleEntity);
                }
                else
                {
                    user.RemoveRole(userRole.Role);
                }
            }

            // Add new role
            user.AssignRole(newRole);
        }

        // Update account status if provided and different from current
        if (!string.IsNullOrWhiteSpace(accountStatus) && user.Account != null)
        {
            var targetStatus = accountStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) 
                ? AccountStatus.Active 
                : AccountStatus.Inactive;

            // Only update if the status is actually changing
            if (user.Account.Status != targetStatus)
            {
                if (targetStatus == AccountStatus.Active)
                    user.Account.ActivateAccount();
                else
                    user.Account.SuspendAccount();
            }
        }

        await _userRepository.UpdateAsync(user);
        return user;
    }

}

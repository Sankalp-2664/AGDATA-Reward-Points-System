using Application.Configuration;
using Application.Interfaces;

namespace Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    JwtSettings jwtSettings) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly JwtSettings _jwtSettings = jwtSettings;

    public async Task<(bool Success, string Token, DateTime ExpiresAt, string? Error)>
    LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            // for debugging only
            Console.WriteLine($"Login failed: user not found for email {email}");
            return (false, string.Empty, default, "Invalid credentials.");
        }

        if (user.Account is null)
        {
            Console.WriteLine($"Login failed: account missing for user {user.Id}");
            return (false, string.Empty, default, "Invalid credentials.");
        }

        var account = user.Account;

        var valid = _passwordHasher.Verify(password, account.PasswordHash, account.PasswordSalt);
        if (!valid)
        {
            Console.WriteLine($"Login failed: wrong password for email {email}");
            return (false, string.Empty, default, "Invalid credentials.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

        return (true, token, expiresAt, null);
    }

}

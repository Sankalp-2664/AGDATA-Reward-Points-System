using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;

namespace Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        var hash = HashInternal(password, salt);
        return (hash, salt);
    }

    public bool Verify(string password, string hash, string salt)
    {
        var computed = HashInternal(password, salt);
        return computed == hash;
    }

    private string HashInternal(string password, string salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);
        var bytes = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}

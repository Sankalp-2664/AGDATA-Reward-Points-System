using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Token, DateTime ExpiresAt, string? Error)>
        LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}

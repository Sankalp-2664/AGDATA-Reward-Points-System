using Domain.Entities.User;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserProfile user);
}

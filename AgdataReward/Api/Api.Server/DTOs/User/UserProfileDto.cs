using Domain.Entities.User;

namespace Api.Server.DTOs.User;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public UserAccountDto? Account { get; set; }

    public static UserProfileDto FromDomain(UserProfile entity)
    {
        return new UserProfileDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId.Value,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email.Value,
            Role = entity.Role.ToString(),
            Account = entity.Account != null ? UserAccountDto.FromDomain(entity.Account) : null
        };
    }
}

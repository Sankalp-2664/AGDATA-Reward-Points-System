using Domain.Entities.User;

namespace Api.Server.DTOs.User;

public class UserAccountDto
{
    public Guid Id { get; set; }
    public int RewardBalance { get; set; }
    public string Status { get; set; } = string.Empty;

    public static UserAccountDto FromDomain(UserAccount entity)
    {
        return new UserAccountDto
        {
            Id = entity.Id,
            RewardBalance = entity.RewardBalance,
            Status = entity.Status.ToString()
        };
    }
}

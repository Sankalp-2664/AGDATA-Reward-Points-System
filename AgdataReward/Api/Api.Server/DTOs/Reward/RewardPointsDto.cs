using Domain.Entities.Reward;

namespace Api.Server.DTOs.Reward;

public class RewardPointsDto
{
    public Guid Id { get; set; }
    public int PointsValue { get; set; }

    public static RewardPointsDto FromDomain(RewardPoints entity)
    {
        return new RewardPointsDto
        {
            Id = entity.Id,
            PointsValue = entity.PointsValue
        };
    }
}

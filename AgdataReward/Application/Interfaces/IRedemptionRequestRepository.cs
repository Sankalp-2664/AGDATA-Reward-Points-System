using Domain.Entities.Redemption;

namespace Application.Interfaces;

public interface IRedemptionRequestRepository
{
    Task<RedemptionRequest?> GetByIdAsync(Guid redemptionId);
    Task AddAsync(RedemptionRequest request);
    Task UpdateAsync(RedemptionRequest process);
    Task<IEnumerable<RedemptionRequest>> GetPendingOrActiveByUserAndProductAsync(
        Guid userId,
        Guid productId,
        IEnumerable<RedemptionRecord> allRedemptionRecords
    );
    Task<IEnumerable<RedemptionRequest>> GetAllPendingAsync();
    Task<IEnumerable<RedemptionRequest>> GetByRedemptionIdsAsync(IEnumerable<Guid> redemptionIds);
}

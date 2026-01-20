using Domain.Entities.Redemption;

namespace Application.Interfaces;

public interface IRedemptionService
{
    Task<RedemptionRecord> RequestRedemptionAsync(Guid userId, Guid productId);
    Task ApproveRedemptionAsync(Guid redemptionId);
    Task RejectRedemptionAsync(Guid redemptionId);
    Task CompleteRedemptionAsync(Guid redemptionId);
    Task<RedemptionRecord?> GetRedemptionByIdAsync(Guid redemptionId);
    Task<IEnumerable<RedemptionRequest>> GetAllPendingRequestsAsync();
}

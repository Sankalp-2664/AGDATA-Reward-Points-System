using Domain.Entities.Redemption;

namespace Application.Interfaces;

public interface IRedemptionRecordRepository
{
    Task<RedemptionRecord?> GetByIdAsync(Guid id);
    Task AddAsync(RedemptionRecord record);
    Task<IEnumerable<RedemptionRecord>> GetAllAsync();
}

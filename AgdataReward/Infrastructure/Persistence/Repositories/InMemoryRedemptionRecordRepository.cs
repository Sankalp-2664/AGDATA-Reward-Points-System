using Application.Interfaces;
using Domain.Entities.Redemption;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryRedemptionRecordRepository : IRedemptionRecordRepository
{
    private readonly List<RedemptionRecord> _records = new();

    public Task<RedemptionRecord?> GetByIdAsync(Guid id)
        => Task.FromResult(_records.FirstOrDefault(r => r.Id == id));

    public Task AddAsync(RedemptionRecord record)
    {
        _records.Add(record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RedemptionRecord>> GetAllAsync()
    {
        return Task.FromResult(_records.AsEnumerable());
    }

    public Task<IEnumerable<RedemptionRecord>> GetByUserIdAsync(Guid userId)
    {
        var userRecords = _records.Where(r => r.UserId == userId).AsEnumerable();
        return Task.FromResult(userRecords);
    }
}

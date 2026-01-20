using Application.Interfaces;
using Domain.Entities.Redemption;
using Domain.Enums;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryRedemptionRequestRepository : IRedemptionRequestRepository
{
    private readonly Dictionary<Guid, RedemptionRequest> _processes = new();


    public Task<RedemptionRequest?> GetByIdAsync(Guid redemptionId)
    {
        _processes.TryGetValue(redemptionId, out var process);
        return Task.FromResult(process);
    }

    public Task AddAsync(RedemptionRequest request)
    {
        _processes[request.RedemptionId] = request;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RedemptionRequest process)
    {
        _processes[process.RedemptionId] = process;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RedemptionRequest>> GetPendingOrActiveByUserAndProductAsync(
        Guid userId,
        Guid productId,
        IEnumerable<RedemptionRecord> allRedemptionRecords)
    {
        // Map redemption records for this user/product
        var recordIds = allRedemptionRecords
            .Where(r => r.UserId == userId && r.ProductId == productId)
            .Select(r => r.Id)
            .ToHashSet();

        var result = _processes.Values
        .Where(r => recordIds.Contains(r.RedemptionId) &&
                (r.Status == RedemptionStatus.Pending || r.Status == RedemptionStatus.Approved))
        .ToList();

        return Task.FromResult<IEnumerable<RedemptionRequest>>(result);
    }

    public Task<IEnumerable<RedemptionRequest>> GetAllPendingAsync()
    {
        var pending = _processes.Values
            .Where(r => r.Status == RedemptionStatus.Pending)
            .ToList();
        return Task.FromResult<IEnumerable<RedemptionRequest>>(pending);
    }

    public Task<IEnumerable<RedemptionRequest>> GetByRedemptionIdsAsync(IEnumerable<Guid> redemptionIds)
    {
        var ids = redemptionIds.ToHashSet();
        var requests = _processes.Values
            .Where(r => ids.Contains(r.RedemptionId))
            .ToList();
        return Task.FromResult<IEnumerable<RedemptionRequest>>(requests);
    }
}

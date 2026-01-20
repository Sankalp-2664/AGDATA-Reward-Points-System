using Application.Interfaces;
using Domain.Entities.Reward;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryRewardTransactionRepository : IRewardTransactionRepository
{
    private readonly List<RewardTransaction> _transactions = new();

    public Task<RewardTransaction?> GetByIdAsync(Guid id)
        => Task.FromResult(_transactions.FirstOrDefault(t => t.UserId == id));

    public Task AddAsync(RewardTransaction transaction)
    {
        _transactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RewardTransaction>> GetByUserIdAsync(Guid userId)
        => Task.FromResult<IEnumerable<RewardTransaction>>(_transactions.Where(t => t.UserId == userId));

    public Task<bool> HasTransactionsForEventAsync(Guid eventId)
    {
        var eventIdString = eventId.ToString();
        return Task.FromResult(_transactions.Any(t => t.Notes.Contains(eventIdString)));
    }
}

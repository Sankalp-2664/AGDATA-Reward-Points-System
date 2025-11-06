using Domain.Entities.Reward;

namespace Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId);
}

using Application.Interfaces;
using Domain.Entities.Reward;

namespace Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IRewardTransactionRepository _transactionRepo;

    public TransactionService(IRewardTransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public async Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId)
        => await _transactionRepo.GetByUserIdAsync(userId);
}

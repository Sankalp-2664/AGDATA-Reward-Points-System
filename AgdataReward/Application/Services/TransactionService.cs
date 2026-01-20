using Application.Interfaces;
using Domain.Entities.Reward;

namespace Application.Services;

public class TransactionService(
    IRewardTransactionRepository transactionRepo,
    IRewardPointsRepository rewardPointsRepo,
    IUserAccountRepository accountRepo) : ITransactionService
{
    private readonly IRewardTransactionRepository _transactionRepo = transactionRepo;
    private readonly IRewardPointsRepository _rewardPointsRepo = rewardPointsRepo;
    private readonly IUserAccountRepository _accountRepo = accountRepo;

    public async Task<RewardPoints> CreateRewardPointsAsync(int pointsValue)
    {
        var rewardPoints = new RewardPoints(Guid.NewGuid(), pointsValue);
        await _rewardPointsRepo.AddAsync(rewardPoints);
        return rewardPoints;
    }

    public async Task<RewardPoints?> GetRewardPointsByIdAsync(Guid id)
    {
        return await _rewardPointsRepo.GetByIdAsync(id);
    }

    public async Task<RewardPoints> UpdateRewardPointsAsync(Guid id, int newPointsValue)
    {
        var rewardPoints = await _rewardPointsRepo.GetByIdAsync(id);
        if (rewardPoints == null)
            throw new InvalidOperationException($"RewardPoints with ID {id} not found.");

        rewardPoints.UpdatePointsValue(newPointsValue);
        await _rewardPointsRepo.UpdateAsync(rewardPoints);
        return rewardPoints;
    }

    public async Task<IEnumerable<RewardPoints>> ListRewardPointsAsync()
    {
        return await _rewardPointsRepo.ListAsync();
    }

    public async Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId)
    {
        // userId is UserProfile.Id, but RewardTransaction.UserId is FK to UserAccount.Id
        // So we need to get the UserAccount first
        var account = await _accountRepo.GetByUserIdAsync(userId);
        if (account == null)
        {
            return Enumerable.Empty<RewardTransaction>();
        }

        // Now query transactions using UserAccount.Id
        return await _transactionRepo.GetByUserIdAsync(account.Id);
    }
}
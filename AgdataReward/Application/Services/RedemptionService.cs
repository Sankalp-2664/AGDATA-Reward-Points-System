using Application.Interfaces;
using Domain.Entities.Redemption;
using Domain.Entities.Reward;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services;

public class RedemptionService(
    IRedemptionRecordRepository recordRepo,
    IRedemptionRequestRepository processRepo,
    IUserAccountRepository accountRepo,
    IProductRepository productRepo,
    IProductInventoryRepository inventoryRepo,
    IRewardPointsRepository rewardPointsRepo,
    IRewardTransactionRepository transactionRepo) : IRedemptionService
{
    private readonly IRedemptionRecordRepository _recordRepo = recordRepo;
    private readonly IRedemptionRequestRepository _processRepo = processRepo;
    private readonly IUserAccountRepository _accountRepo = accountRepo;
    private readonly IProductRepository _productRepo = productRepo;
    private readonly IProductInventoryRepository _inventoryRepo = inventoryRepo;
    private readonly IRewardPointsRepository _rewardPointsRepo = rewardPointsRepo;
    private readonly IRewardTransactionRepository _transactionRepo = transactionRepo;
    
    public async Task<RedemptionRecord> RequestRedemptionAsync(Guid userId, Guid productId)
    { 
        var product = await _productRepo.GetByIdAsync(productId) ?? throw new ArgumentException("Invalid product.");
        var inventory = await _inventoryRepo.GetByProductIdAsync(productId) ?? throw new ArgumentException("No inventory.");
        var rewardPoints = await _rewardPointsRepo.GetByIdAsync(product.RewardPointsId) ?? throw new ArgumentException("Invalid reward points configuration.");
        var account = await _accountRepo.GetByUserIdAsync(userId) ?? throw new ArgumentException("Invalid user.");

        if (account.RewardBalance < rewardPoints.PointsValue)
            throw new InsufficientPointsException(account.RewardBalance, rewardPoints.PointsValue);

        if (inventory.StockQuantity <= 0)
            throw new InvalidRedemptionException("Product is out of stock.");

        // Check for existing pending/approved requests for same user and product
        var allRecords = await _recordRepo.GetAllAsync();
        var existingRequests = await _processRepo.GetPendingOrActiveByUserAndProductAsync(userId, productId, allRecords);
        if (existingRequests.Any())
        {
            throw new InvalidRedemptionException("You already have a pending or approved redemption request for this product.");
        }

        // Create redemption record first
        var record = new RedemptionRecord(Guid.NewGuid(), userId, productId);
        await _recordRepo.AddAsync(record);

        // Create redemption request (use AddAsync, not UpdateAsync for new entities)
        var request = new RedemptionRequest(record.Id, rewardPoints.PointsValue);
        await _processRepo.AddAsync(request);

        // Deduct points immediately when request is created
        // Note: RewardTransaction.UserId is FK to UserAccount.Id (not UserProfile.Id)
        // Note: RewardTransaction.RedemptionId is FK to RedemptionRequests.Id (not RedemptionRecords.Id)
        var transaction = new RewardTransaction(
            account.Id,  // Use UserAccount.Id, not UserProfile.Id
            -rewardPoints.PointsValue,
            $"Redemption request for product {product.Name}",
            TransactionType.Debit,
            redemptionId: request.Id  // Use RedemptionRequest.Id (FK target), not RedemptionRecord.Id
        );

        account.RedeemPoints(rewardPoints.PointsValue, transaction);
        await _transactionRepo.AddAsync(transaction);
        await _accountRepo.UpdateAsync(account);

        // Reduce stock immediately when request is created
        inventory.ReduceStock(1);
        await _inventoryRepo.UpdateAsync(inventory);

        return record;
    }

    public async Task ApproveRedemptionAsync(Guid redemptionId)
    {
        var process = await _processRepo.GetByIdAsync(redemptionId) ?? throw new ArgumentException("Invalid redemption.");
        process.Approve();
        await _processRepo.UpdateAsync(process);
    }

    public async Task RejectRedemptionAsync(Guid redemptionId)
    {
        var process = await _processRepo.GetByIdAsync(redemptionId) ?? throw new ArgumentException("Invalid redemption.");
        
        // Get related record to revert points and stock
        // Note: redemptionId is RedemptionRequest.Id, use process.RedemptionId to get RedemptionRecord.Id
        var record = await _recordRepo.GetByIdAsync(process.RedemptionId)
                     ?? throw new ArgumentException("Invalid redemption record.");
        
        var product = await _productRepo.GetByIdAsync(record.ProductId)
                      ?? throw new ArgumentException("Invalid product.");
        
        var rewardPoints = await _rewardPointsRepo.GetByIdAsync(product.RewardPointsId)
                           ?? throw new ArgumentException("Invalid reward points configuration.");
        
        var account = await _accountRepo.GetByUserIdAsync(record.UserId)
                      ?? throw new ArgumentException("Invalid user account.");
        
        var inventory = await _inventoryRepo.GetByProductIdAsync(record.ProductId)
                        ?? throw new ArgumentException("Invalid inventory.");
        
        // Revert points back to the user
        // Note: RewardTransaction.UserId is FK to UserAccount.Id (not UserProfile.Id)
        var revertTransaction = new RewardTransaction(
            account.Id,  // Use UserAccount.Id, not UserProfile.Id
            rewardPoints.PointsValue,
            $"Redemption request rejected for product {product.Name}",
            TransactionType.Credit,
            redemptionId: redemptionId
        );
        
        account.AddPoints(rewardPoints.PointsValue, revertTransaction);
        await _transactionRepo.AddAsync(revertTransaction);
        await _accountRepo.UpdateAsync(account);
        
        // Revert stock back (add 1 back to inventory)
        inventory.IncreaseStock(1);
        await _inventoryRepo.UpdateAsync(inventory);
        
        // Mark as rejected
        process.Reject();
        await _processRepo.UpdateAsync(process);
    }

    public async Task CompleteRedemptionAsync(Guid redemptionId)
    {
        var process = await _processRepo.GetByIdAsync(redemptionId)
                      ?? throw new ArgumentException("Invalid redemption.");

        if (process.Status != RedemptionStatus.Approved)
            throw new InvalidOperationException("Redemption must be approved before completion.");

        // Stock was already reduced when request was created, and points were already deducted
        // Just mark as complete
        process.MarkCompleted();
        await _processRepo.UpdateAsync(process);
    }

    public async Task<RedemptionRecord?> GetRedemptionByIdAsync(Guid redemptionId)
    {
        return await _recordRepo.GetByIdAsync(redemptionId);
    }

    public async Task<IEnumerable<RedemptionRequest>> GetAllPendingRequestsAsync()
    {
        return await _processRepo.GetAllPendingAsync();
    }
}

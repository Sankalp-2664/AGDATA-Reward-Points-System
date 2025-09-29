using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RedemptionService : IRedemptionService
    {
        private readonly IRedemptionRecordRepository _recordRepo;
        private readonly IRedemptionProcessRepository _processRepo;
        private readonly IUserAccountRepository _accountRepo;
        private readonly IProductRepository _productRepo;
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IRewardPointsRepository _rewardPointsRepo;
        private readonly IRewardTransactionRepository _transactionRepo;

        public RedemptionService(
            IRedemptionRecordRepository recordRepo,
            IRedemptionProcessRepository processRepo,
            IUserAccountRepository accountRepo,
            IProductRepository productRepo,
            IProductInventoryRepository inventoryRepo,
            IRewardPointsRepository rewardPointsRepo,
            IRewardTransactionRepository transactionRepo)
        {
            _recordRepo = recordRepo;
            _processRepo = processRepo;
            _accountRepo = accountRepo;
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _rewardPointsRepo = rewardPointsRepo;
            _transactionRepo = transactionRepo;
        }

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

            // Create record + process
            var record = new RedemptionRecord(Guid.NewGuid(), userId, productId);
            await _recordRepo.AddAsync(record);

            var process = new RedemptionProcess(record.Id, rewardPoints.PointsValue);
            await _processRepo.UpdateAsync(process);

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
            process.Reject();
            await _processRepo.UpdateAsync(process);
        }

        public async Task CompleteRedemptionAsync(Guid redemptionId)
        {
            var process = await _processRepo.GetByIdAsync(redemptionId)
                          ?? throw new ArgumentException("Invalid redemption.");

            if (process.Status != RedemptionStatus.Approved)
                throw new InvalidOperationException("Redemption must be approved before completion.");

            // Get related record
            var record = await _recordRepo.GetByIdAsync(redemptionId)
                         ?? throw new ArgumentException("Invalid redemption record.");

            var product = await _productRepo.GetByIdAsync(record.ProductId)
                          ?? throw new ArgumentException("Invalid product.");
            var rewardPoints = await _rewardPointsRepo.GetByIdAsync(product.RewardPointsId)
                               ?? throw new ArgumentException("Invalid reward points configuration.");

            // Deduct points from user account
            var account = await _accountRepo.GetByUserIdAsync(record.UserId)
                          ?? throw new ArgumentException("Invalid user account.");

            var transaction = new RewardTransaction(
                Guid.NewGuid(),
                account.UserId,
                -rewardPoints.PointsValue,
                $"Redeemed product {product.Name}",
                redemptionId: redemptionId
            );

            account.RedeemPoints(rewardPoints.PointsValue, transaction);
            await _accountRepo.UpdateAsync(account);
            await _transactionRepo.AddAsync(transaction);

            // Update product inventory
            var inventory = await _inventoryRepo.GetByProductIdAsync(product.Id)
                            ?? throw new ArgumentException("Invalid inventory.");
            inventory.ReduceStock(1);
            await _inventoryRepo.UpdateAsync(inventory);

            // Mark process complete
            process.MarkCompleted();
            await _processRepo.UpdateAsync(process);
        }
    }
}

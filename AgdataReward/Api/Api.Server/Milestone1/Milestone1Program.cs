using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;

namespace Api.Server.Milestone1
{
    public static class Milestone1Program
    {
        public static async Task Run()
        {
            Console.WriteLine("--- Milestone 1 Demo Starting ---");

            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var eventDefRepo = new InMemoryEventDefinitionRepository();
            var eventRuleRepo = new InMemoryEventRewardRuleRepository();
            var eventInstanceRepo = new InMemoryEventInstanceRepository();
            var productRepo = new InMemoryProductRepository();
            var inventoryRepo = new InMemoryProductInventoryRepository();
            var rewardPointsRepo = new InMemoryRewardPointsRepository();
            var transactionRepo = new InMemoryRewardTransactionRepository();
            var redemptionRepo = new InMemoryRedemptionRecordRepository();
            var redemptionProcessRepo = new InMemoryRedemptionProcessRepository();

            var eventService = new EventService(
                eventDefRepo, eventRuleRepo, eventInstanceRepo,
                accountRepo, transactionRepo, rewardPointsRepo);

            var redemptionService = new RedemptionService(
                redemptionRepo, redemptionProcessRepo, accountRepo,
                productRepo, inventoryRepo, rewardPointsRepo, transactionRepo);

            var user = new UserProfile(Guid.NewGuid(), "E123", "user@mail.com", "Sankalp", "Chakre");
            await userRepo.AddAsync(user);

            var account = new UserAccount(user.Id);
            await accountRepo.AddAsync(account);

            var eventDef = await eventService.CreateEventAsync("HACK", "Hackathon");
            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await rewardPointsRepo.AddAsync(rewardPoints);
            await eventService.AddRewardRuleAsync(eventDef.Id, 1, rewardPoints.Id);
            var instance = new EventInstance(Guid.NewGuid(), eventDef.Id);
            await eventInstanceRepo.AddAsync(instance);

            await eventService.AssignWinnerAsync(instance.Id, user.Id, 1);

            var product = new ProductInfo(Guid.NewGuid(), "SKU1", "Coffee Mug", rewardPoints.Id);
            await productRepo.AddAsync(product);
            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var redemption = await redemptionService.RequestRedemptionAsync(user.Id, product.Id);
            await redemptionService.ApproveRedemptionAsync(redemption.Id);
            await redemptionService.CompleteRedemptionAsync(redemption.Id);

            var updatedAccount = await accountRepo.GetByUserIdAsync(user.Id);

            Console.WriteLine($"User {user.FirstName} now has balance: {updatedAccount!.RewardBalance}");
            Console.WriteLine($"Product {product.Name} remaining stock: {(await inventoryRepo.GetByProductIdAsync(product.Id))!.StockQuantity}");
        }
    }
}

using Application.Interfaces;
using Application.Services;
using Domain.Entities.Event;
using Domain.Entities.Product;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;   // PasswordHasher

namespace Api.Server.Milestone1;

public static class Milestone1Program
{
    public static async Task Run()
    {
        Console.WriteLine("--- Milestone 1 Demo Starting ---");

        // =========================
        // In-memory repositories
        // =========================
        var userRepo = new InMemoryUserRepository();
        var accountRepo = new InMemoryUserAccountRepository();
        var roleRepo = new InMemoryRoleRepository();

        var eventDefRepo = new InMemoryEventDefinitionRepository();
        var eventRuleRepo = new InMemoryEventRewardRuleRepository();
        var eventInstanceRepo = new InMemoryEventInstanceRepository();

        var productRepo = new InMemoryProductRepository();
        var inventoryRepo = new InMemoryProductInventoryRepository();

        var rewardPointsRepo = new InMemoryRewardPointsRepository();
        var transactionRepo = new InMemoryRewardTransactionRepository();

        var redemptionRepo = new InMemoryRedemptionRecordRepository();
        var redemptionRequestRepo = new InMemoryRedemptionRequestRepository();

        // =========================
        // Seed Roles
        // =========================
        await roleRepo.AddAsync(new Role("Admin"));
        await roleRepo.AddAsync(new Role("User"));

        // =========================
        // Password hasher
        // =========================
        IPasswordHasher passwordHasher = new PasswordHasher();

        // =========================
        // Services
        // =========================
        IUserService userService = new UserService(
            userRepo,
            accountRepo,
            roleRepo,
            passwordHasher);

        IEventService eventService = new EventService(
            eventDefRepo,
            eventRuleRepo,
            eventInstanceRepo,
            accountRepo,
            transactionRepo,
            rewardPointsRepo);

        IRedemptionService redemptionService = new RedemptionService(
            redemptionRepo,
            redemptionRequestRepo,
            accountRepo,
            productRepo,
            inventoryRepo,
            rewardPointsRepo,
            transactionRepo);

        // =========================
        // 1. Register User
        // =========================
        var user = await userService.RegisterUserAsync(
            "E123",
            "user@mail.com",
            "Sankalp",
            "Chakre",
            "User",
            "User@123"
        );

        Console.WriteLine($"Registered user: {user.FirstName} ({user.Email.Value})");

        // =========================
        // 2. Create Event
        // =========================
        var eventDef = await eventService.CreateEventAsync("HACK", "Hackathon", DateTime.Now, DateTime.Now.AddDays(7));
        Console.WriteLine($"Created event: {eventDef.Title}");

        // =========================
        // 3. Add Reward Points + Rule
        // =========================
        var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
        await rewardPointsRepo.AddAsync(rewardPoints);

        await eventService.AddRewardRuleAsync(
            eventDef.Id,
            rank: 1,
            rewardPointsId: rewardPoints.Id);

        // =========================
        // 4. Add Event Instance
        // =========================
        var instance = new EventInstance(Guid.NewGuid(), eventDef.Id);
        await eventInstanceRepo.AddAsync(instance);

        // =========================
        // 5. Assign Winner
        // =========================
        await eventService.AssignWinnerAsync(
            instance.Id,
            user.Id,
            rank: 1);

        Console.WriteLine($"{user.FirstName} assigned as winner for event instance.");

        // =========================
        // 6. Product + Inventory
        // =========================
        var product = new ProductInformation(
            Guid.NewGuid(),
            new SKU("SKU1"),
            "Coffee Mug",
            rewardPoints.Id);

        await productRepo.AddAsync(product);

        var inventory = new ProductInventory(
            Guid.NewGuid(),
            product.Id,
            5);

        await inventoryRepo.AddAsync(inventory);

        // =========================
        // 7. Request Redemption
        // =========================
        var redemption = await redemptionService.RequestRedemptionAsync(
            user.Id,
            product.Id);

        Console.WriteLine($"Redemption requested for product: {product.Name}");

        // =========================
        // 8. Approve + Complete Redemption
        // =========================
        await redemptionService.ApproveRedemptionAsync(redemption.Id);
        await redemptionService.CompleteRedemptionAsync(redemption.Id);

        Console.WriteLine($"Redemption completed for product: {product.Name}");

        // =========================
        // 9. Final State
        // =========================
        var updatedAccount = await userService.GetUserAccountAsync(user.Id);
        var updatedInventory = await inventoryRepo.GetByProductIdAsync(product.Id);

        Console.WriteLine($"User {user.FirstName} balance: {updatedAccount!.RewardBalance}");
        Console.WriteLine($"Product '{product.Name}' remaining stock: {updatedInventory!.StockQuantity}");

        Console.WriteLine("--- Milestone 1 Demo Completed ---");
    }
}

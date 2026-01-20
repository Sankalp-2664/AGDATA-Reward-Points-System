using Application.Interfaces;
using Domain.Entities.User;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RewardDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        Console.WriteLine("DbSeeder: starting...");

        // ===============================
        // 1. Seed Roles
        // ===============================
        if (!await context.Set<Role>().AnyAsync())
        {
            var adminRole = new Role("Admin");
            var userRole = new Role("User");

            await context.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Roles seeded successfully.");
        }

        // ===============================
        // 2. Seed Admin User
        // ===============================
        if (!await context.UserProfiles.AnyAsync())
        {
            var adminRole = await context.Set<Role>()
                .FirstAsync(r => r.Name == "Admin");

            var email = new Email("Sankalp.Chkare@agdata.com");
            var employeeId = new EmployeeId("EMP12563");

            var adminUser = new UserProfile(
                employeeId,
                email,
                "Sankalp",
                "Chakre"
            );

            // Create account
            var account = new UserAccount(adminUser.Id);

            var passwordResult = passwordHasher.Hash("Sankalp2664");
            account.SetCredentials(passwordResult.Hash, passwordResult.Salt);

            adminUser.AttachAccount(account);

            // Assign Admin role
            adminUser.AssignRole(adminRole);

            await context.UserProfiles.AddAsync(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Admin user seeded successfully.");
            Console.WriteLine("  Email: Sankalp.Chkare@agdata.com");
            Console.WriteLine("  Password: Sankalp2664");
            Console.WriteLine("  Employee ID: EMP12563");
        }
        else
        {
            Console.WriteLine("ℹ Users already exist. Skipping user seeding.");
        }

        // ===============================
        // 3. Seed Reward Points
        // ===============================
        if (!await context.Set<Domain.Entities.Reward.RewardPoints>().AnyAsync())
        {
            var rewardPoints = new[]
            {
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 100),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 200),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 300),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 500),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 1000),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 2000),
                new Domain.Entities.Reward.RewardPoints(Guid.NewGuid(), 5000)
            };

            await context.AddRangeAsync(rewardPoints);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Reward points seeded successfully.");
            Console.WriteLine($"  Created {rewardPoints.Length} reward point configurations: 100, 200, 300, 500, 1000, 2000, 5000");
        }
        else
        {
            Console.WriteLine("ℹ Reward points already exist. Skipping reward points seeding.");
        }
    }
}


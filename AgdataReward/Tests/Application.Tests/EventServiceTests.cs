using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Application.Tests
{
    public class EventServiceTests
    {
        [Fact]
        public async Task AssignWinner_ShouldAddPointsAndTransaction()
        {
            // Arrange
            var defRepo = new InMemoryEventDefinitionRepository();
            var ruleRepo = new InMemoryEventRewardRuleRepository();
            var instRepo = new InMemoryEventInstanceRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var transactionRepo = new InMemoryRewardTransactionRepository();
            var pointsRepo = new InMemoryRewardPointsRepository();

            var eventService = new EventService(defRepo, ruleRepo, instRepo, accountRepo, transactionRepo, pointsRepo);

            var userAccount = new UserAccount(Guid.NewGuid());
            await accountRepo.AddAsync(userAccount);

            var def = await eventService.CreateEventAsync("HACK", "Hackathon");
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            await eventService.AddRewardRuleAsync(def.Id, 1, rp.Id);
            var instance = new EventInstance(Guid.NewGuid(), def.Id);
            await instRepo.AddAsync(instance);

            // Act
            await eventService.AssignWinnerAsync(instance.Id, userAccount.UserId, 1);

            // Assert
            var updated = await accountRepo.GetByUserIdAsync(userAccount.UserId);
            Assert.Equal(100, updated!.RewardBalance);

            var transactions = await transactionRepo.GetByUserIdAsync(userAccount.UserId);
            Assert.Single(transactions);
        }
    }
}

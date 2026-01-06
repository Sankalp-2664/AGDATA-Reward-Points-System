using Application.Services;
using Domain.Entities.Event;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Tests.Application.Tests.Services;

public class EventServiceTests
{
    private EventService BuildService(
        out InMemoryEventDefinitionRepository defRepo,
        out InMemoryEventRewardRuleRepository ruleRepo,
        out InMemoryEventInstanceRepository instRepo,
        out InMemoryUserAccountRepository accountRepo,
        out InMemoryRewardTransactionRepository txRepo,
        out InMemoryRewardPointsRepository pointsRepo)
    {
        defRepo = new InMemoryEventDefinitionRepository();
        ruleRepo = new InMemoryEventRewardRuleRepository();
        instRepo = new InMemoryEventInstanceRepository();
        accountRepo = new InMemoryUserAccountRepository();
        txRepo = new InMemoryRewardTransactionRepository();
        pointsRepo = new InMemoryRewardPointsRepository();
        return new EventService(defRepo, ruleRepo, instRepo, accountRepo, txRepo, pointsRepo);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldReturnEvent_WhenValid()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("CODE1", "Title 1");

        Assert.NotNull(ev);
        Assert.Equal("CODE1", ev.Code);
        Assert.Equal("Title 1", ev.Title);

        var fromRepo = await defRepo.GetByIdAsync(ev.Id);
        Assert.NotNull(fromRepo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateEventAsync_ShouldThrow_IfCodeEmpty(string code)
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateEventAsync(code, "Some Title"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateEventAsync_ShouldThrow_IfTitleEmpty(string title)
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateEventAsync("CODEX", title));
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldThrow_IfEmptyGuid()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.GetEventByIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task ListEventsAsync_ShouldReturnAllEvents()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var e1 = await service.CreateEventAsync("C1", "Title1");
        var e2 = await service.CreateEventAsync("C2", "Title2");

        var list = await service.ListEventsAsync();
        Assert.Equal(2, list.Count());
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldAwardPoints_WhenValid()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var userId = Guid.NewGuid();
        var userAccount = new UserAccount(userId);
        await accRepo.AddAsync(userAccount);

        var ev = await service.CreateEventAsync("E1", "Ev 1");
        var rp = new RewardPoints(Guid.NewGuid(), 150);
        await ptsRepo.AddAsync(rp);
        await service.AddRewardRuleAsync(ev.Id, rank: 1, rewardPointsId: rp.Id);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        await service.AssignWinnerAsync(inst.Id, userId, rank: 1);

        var updated = await accRepo.GetByUserIdAsync(userId);
        Assert.Equal(150, updated!.RewardBalance);

        var txs = await txRepo.GetByUserIdAsync(userId);
        Assert.Single(txs);
        Assert.Equal(TransactionType.Credit, txs.First().TransactionType);
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldThrow_IfInstanceInvalid()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignWinnerAsync(Guid.NewGuid(), Guid.NewGuid(), 1));
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldThrow_IfNoRuleForRank()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EV2", "Event2");
        var rp = new RewardPoints(Guid.NewGuid(), 200);
        await ptsRepo.AddAsync(rp);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        var userAccount = new UserAccount(Guid.NewGuid());
        await accRepo.AddAsync(userAccount);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignWinnerAsync(inst.Id, userAccount.Id, 2));
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldThrow_IfRewardPointsNotFound()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EV3", "Event3");
        await service.AddRewardRuleAsync(ev.Id, 1, Guid.NewGuid());

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        var userAccount = new UserAccount(Guid.NewGuid());
        await accRepo.AddAsync(userAccount);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignWinnerAsync(inst.Id, userAccount.Id, 1));
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldThrow_IfUserAccountMissing()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EV4", "Event4");
        var rp = new RewardPoints(Guid.NewGuid(), 300);
        await ptsRepo.AddAsync(rp);
        await service.AddRewardRuleAsync(ev.Id, 1, rp.Id);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AssignWinnerAsync(inst.Id, Guid.NewGuid(), 1));
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldThrow_IfRankIsZeroOrNegative()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EV5", "Event5");
        var rp = new RewardPoints(Guid.NewGuid(), 100);
        await ptsRepo.AddAsync(rp);
        await service.AddRewardRuleAsync(ev.Id, 1, rp.Id);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        var userAccount = new UserAccount(Guid.NewGuid());
        await accRepo.AddAsync(userAccount);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AssignWinnerAsync(inst.Id, userAccount.Id, 0));
        await Assert.ThrowsAsync<ArgumentException>(() => service.AssignWinnerAsync(inst.Id, userAccount.Id, -1));
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldAllowMultipleAssignments_ForSameInstanceAndUser()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EV6", "Event6");
        var rp1 = new RewardPoints(Guid.NewGuid(), 50);
        var rp2 = new RewardPoints(Guid.NewGuid(), 30);
        await ptsRepo.AddAsync(rp1);
        await ptsRepo.AddAsync(rp2);

        await service.AddRewardRuleAsync(ev.Id, 1, rp1.Id);
        await service.AddRewardRuleAsync(ev.Id, 2, rp2.Id);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        var userAccount = new UserAccount(Guid.NewGuid());
        await accRepo.AddAsync(userAccount);

        await service.AssignWinnerAsync(inst.Id, userAccount.UserId, 1);
        await service.AssignWinnerAsync(inst.Id, userAccount.UserId, 2);

        var updated = await accRepo.GetByUserIdAsync(userAccount.UserId);
        Assert.Equal(80, updated!.RewardBalance);

        var txs = await txRepo.GetByUserIdAsync(userAccount.UserId);
        Assert.Equal(2, txs.Count());
    }

    [Fact]
    public async Task CreateEventAsync_ShouldTrimCodeAndTitle()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("  CODETRIM  ", "  Title Trim  ");

        Assert.Equal("CODETRIM", ev.Code);
        Assert.Equal("Title Trim", ev.Title);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowException_OnDuplicateCode()
    {
        var service = BuildService(
        out var defRepo, out var ruleRepo, out var instRepo,
        out var accRepo, out var txRepo, out var ptsRepo);

        await service.CreateEventAsync("DUPCODE", "Title 1");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateEventAsync("DUPCODE", "Title 2"));

        Assert.Equal("Duplicate event code not allowed", ex.Message);
    }

    [Fact]
    public async Task AssignWinnerAsync_ShouldHandleLargePointsValue()
    {
        var service = BuildService(out var defRepo, out var ruleRepo, out var instRepo, out var accRepo, out var txRepo, out var ptsRepo);

        var ev = await service.CreateEventAsync("EVLARGE", "Large Points Event");
        var largePoints = int.MaxValue;
        var rp = new RewardPoints(Guid.NewGuid(), largePoints);
        await ptsRepo.AddAsync(rp);
        await service.AddRewardRuleAsync(ev.Id, 1, rp.Id);

        var inst = new EventInstance(Guid.NewGuid(), ev.Id);
        await instRepo.AddAsync(inst);

        var userAccount = new UserAccount(Guid.NewGuid());
        await accRepo.AddAsync(userAccount);

        await service.AssignWinnerAsync(inst.Id, userAccount.UserId, 1);

        var updated = await accRepo.GetByUserIdAsync(userAccount.UserId);
        Assert.Equal(largePoints, updated!.RewardBalance);

        var txs = await txRepo.GetByUserIdAsync(userAccount.UserId);
        Assert.Single(txs);
        Assert.Equal(largePoints, txs.First().PointsDelta);
    }
}

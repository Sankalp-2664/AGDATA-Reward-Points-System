using Application.Interfaces;
using Domain.Entities.Event;
using Domain.Entities.Reward;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventDefinitionRepository _definitionRepo;
        private readonly IEventRewardRuleRepository _ruleRepo;
        private readonly IEventInstanceRepository _instanceRepo;
        private readonly IUserAccountRepository _accountRepo;
        private readonly IRewardTransactionRepository _transactionRepo;
        private readonly IRewardPointsRepository _rewardPointsRepo;

        public EventService(
            IEventDefinitionRepository definitionRepo,
            IEventRewardRuleRepository ruleRepo,
            IEventInstanceRepository instanceRepo,
            IUserAccountRepository accountRepo,
            IRewardTransactionRepository transactionRepo,
            IRewardPointsRepository rewardPointsRepo) 
        {
            _definitionRepo = definitionRepo;
            _ruleRepo = ruleRepo;
            _instanceRepo = instanceRepo;
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _rewardPointsRepo = rewardPointsRepo;
        }

        public async Task<EventDefinition> CreateEventAsync(string code, string title)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ValidationException("Event code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Event title cannot be null or empty.");

            var definition = new EventDefinition(Guid.NewGuid(), code.Trim(), title.Trim());
            await _definitionRepo.AddAsync(definition);
            return definition;
        }

        public async Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId)
        {
            var rule = new EventRewardRule(Guid.NewGuid(), eventId, rank, rewardPointsId);
            await _ruleRepo.AddAsync(rule);
        }

        public async Task<EventDefinition?> GetEventByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("Event ID cannot be empty.");
            return await _definitionRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<EventDefinition>> ListEventsAsync()
        {
            return await _definitionRepo.ListAsync();
        }


        /// <summary>
        /// eventInstanceId = EventInstance.Id (the occurrence). Assigns a winner for that occurrence.
        /// </summary>
        public async Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank)
        {

            // 1) load the event instance (occurrence)
            var instance = await _instanceRepo.GetByIdAsync(eventInstanceId)
                ?? throw new ArgumentException("Invalid event instance id.");

            // 2) load rules for the event definition (instance.EventId)
            var rules = await _ruleRepo.GetByEventIdAsync(instance.EventId);
            var rewardRule = rules.FirstOrDefault(r => r.Rank == rank);
            if (rewardRule == null)
                throw new ArgumentException("No reward rule for this rank.");

            // 3) resolve numeric points from RewardPoints
            var rewardPoints = await _rewardPointsRepo.GetByIdAsync(rewardRule.RewardPointsId)
                ?? throw new ArgumentException("Reward points configuration not found.");
            var pointsToAward = rewardPoints.PointsValue;

            // 4) get user account
            var account = await _accountRepo.GetByUserIdAsync(userId)
                ?? throw new ArgumentException("Invalid user.");

            // 5) create transaction and persist it
            var transaction = new RewardTransaction(
                userId,
                rewardPoints.PointsValue,
                $"Earned from event {instance.EventId} (instance {instance.Id})",
                TransactionType.Credit,
                eventId: instance.EventId
            );

            await _transactionRepo.AddAsync(transaction);

            // 6) update account through domain method (which also records the transaction inside the aggregate)
            account.AddPoints(rewardPoints.PointsValue, transaction);
            await _accountRepo.UpdateAsync(account);
        }
    }
}

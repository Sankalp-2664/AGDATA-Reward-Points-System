using Application.Interfaces;
using Domain.Entities.Redemption;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryRedemptionRequestRepository : IRedemptionRequestRepository
    {
        private readonly Dictionary<Guid, RedemptionRequest> _processes = new();


        public Task<RedemptionRequest?> GetByIdAsync(Guid redemptionId)
        {
            _processes.TryGetValue(redemptionId, out var process);
            return Task.FromResult(process);
        }

        public Task UpdateAsync(RedemptionRequest process)
        {
            _processes[process.RedemptionId] = process;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RedemptionRequest>> GetPendingOrActiveByUserAndProductAsync(
            Guid userId,
            Guid productId,
            IEnumerable<RedemptionRecord> allRedemptionRecords)
        {
            // Map redemption records for this user/product
            var recordIds = allRedemptionRecords
                .Where(r => r.UserId == userId && r.ProductId == productId)
                .Select(r => r.Id)
                .ToHashSet();

            var result = _processes.Values
        .Where(r => recordIds.Contains(r.RedemptionId) &&
                    (r.Status == RedemptionStatus.Pending || r.Status == RedemptionStatus.Approved))
        .ToList();

            return Task.FromResult<IEnumerable<RedemptionRequest>>(result);
        }
    }
}

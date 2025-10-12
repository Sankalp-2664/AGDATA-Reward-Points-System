using Application.Interfaces;
using Domain.Entities.Redemption;
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
    }
}

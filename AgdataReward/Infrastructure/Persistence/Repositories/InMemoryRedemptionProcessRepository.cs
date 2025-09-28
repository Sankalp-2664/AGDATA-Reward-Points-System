using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryRedemptionProcessRepository : IRedemptionProcessRepository
    {
        private readonly Dictionary<Guid, RedemptionProcess> _processes = new();

        public Task<RedemptionProcess?> GetByIdAsync(Guid redemptionId)
        {
            _processes.TryGetValue(redemptionId, out var process);
            return Task.FromResult(process);
        }

        public Task UpdateAsync(RedemptionProcess process)
        {
            _processes[process.RedemptionId] = process;
            return Task.CompletedTask;
        }
    }
}

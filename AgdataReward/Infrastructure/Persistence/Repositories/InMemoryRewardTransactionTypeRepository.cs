using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryRewardTransactionTypeRepository : IRewardTransactionTypeRepository
    {
        private readonly List<RewardTransactionType> _types = new();

        public Task<RewardTransactionType?> GetByIdAsync(Guid id)
            => Task.FromResult(_types.FirstOrDefault(t => t.Id == id));

        public Task<IEnumerable<RewardTransactionType>> ListAsync()
            => Task.FromResult<IEnumerable<RewardTransactionType>>(_types);
    }
}

using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryRedemptionRecordRepository : IRedemptionRecordRepository
    {
        private readonly List<RedemptionRecord> _records = new();

        public Task<RedemptionRecord?> GetByIdAsync(Guid id)
            => Task.FromResult(_records.FirstOrDefault(r => r.Id == id));

        public Task AddAsync(RedemptionRecord record)
        {
            _records.Add(record);
            return Task.CompletedTask;
        }
    }
}

using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRedemptionRecordRepository
    {
        Task<RedemptionRecord?> GetByIdAsync(Guid id);
        Task AddAsync(RedemptionRecord record);
    }
}

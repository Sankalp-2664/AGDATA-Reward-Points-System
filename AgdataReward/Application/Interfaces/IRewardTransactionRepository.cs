using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRewardTransactionRepository
    {
        Task<RewardTransaction?> GetByIdAsync(Guid id);
        Task AddAsync(RewardTransaction transaction);
        Task<IEnumerable<RewardTransaction>> GetByUserIdAsync(Guid userId);
    }
}

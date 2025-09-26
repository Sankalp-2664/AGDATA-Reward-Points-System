using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRewardTransactionTypeRepository
    {
        Task<RewardTransactionType?> GetByIdAsync(Guid id);
        Task<IEnumerable<RewardTransactionType>> ListAsync();
    }
}

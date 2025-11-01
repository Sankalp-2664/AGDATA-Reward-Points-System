using Domain.Entities.Redemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRedemptionRequestRepository
    {
        Task<RedemptionRequest?> GetByIdAsync(Guid redemptionId);
        Task UpdateAsync(RedemptionRequest process);
        Task<IEnumerable<RedemptionRequest>> GetPendingOrActiveByUserAndProductAsync(
            Guid userId,
            Guid productId,
            IEnumerable<RedemptionRecord> allRedemptionRecords
        );

    }
}

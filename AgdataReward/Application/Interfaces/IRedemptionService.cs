using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRedemptionService
    {
        Task<RedemptionRecord> RequestRedemptionAsync(Guid userId, Guid productId);
        Task ApproveRedemptionAsync(Guid redemptionId);
        Task RejectRedemptionAsync(Guid redemptionId);
        Task CompleteRedemptionAsync(Guid redemptionId);
    }
}

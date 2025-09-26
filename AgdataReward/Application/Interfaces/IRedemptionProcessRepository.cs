using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRedemptionProcessRepository
    {
        Task<RedemptionProcess?> GetByIdAsync(Guid redemptionId);
        Task UpdateAsync(RedemptionProcess process);
    }
}

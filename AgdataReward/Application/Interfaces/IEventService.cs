using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventService
    {
        Task<EventDefinition> CreateEventAsync(string code, string title);
        Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId);
        Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank);
    }
}

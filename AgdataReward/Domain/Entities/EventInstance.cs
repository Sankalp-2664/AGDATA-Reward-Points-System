using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class EventInstance
{
    public Guid Id { get; }
    public Guid EventId { get; }
    public Guid? WinnerUserId { get; private set; }
    public int? Rank { get; private set; }

    public EventInstance(Guid id, Guid eventId)
    {
        Id = id;
        EventId = eventId;
    }

    public void AssignWinner(Guid userId, int rank)
    {
        WinnerUserId = userId;
        Rank = rank;
    }
}


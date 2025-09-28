using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class RewardTransaction
{
    public Guid TransactionId { get; }
    public Guid UserId { get; }
    public int PointsDelta { get; }
    public string Notes { get; }
    public Guid? EventId { get; }
    public Guid? RedemptionId { get; }

    public RewardTransaction(Guid transactionId, Guid userId, int pointsDelta, string notes, Guid? eventId = null, Guid? redemptionId = null)
    {
        if (pointsDelta == 0) throw new ArgumentException("PointsDelta cannot be zero.");
        if (string.IsNullOrWhiteSpace(notes)) throw new ArgumentException("Notes cannot be empty.");

        TransactionId = transactionId;
        UserId = userId;
        PointsDelta = pointsDelta;
        Notes = notes;
        EventId = eventId;
        RedemptionId = redemptionId;
    }
}


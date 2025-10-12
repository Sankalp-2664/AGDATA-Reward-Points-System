using Domain.Entities.User;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Reward
{
    /// <summary>
    /// Represents a single reward points transaction (credit or debit) for a user account.
    /// </summary>
    public class RewardTransaction
    {
        public Guid Id { get; private set; } // Primary Key
        public Guid UserId { get; private set; } // FK to UserAccount
        public int PointsDelta { get; private set; }
        public string Notes { get; private set; } = string.Empty;
        public TransactionType TransactionType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid? EventId { get; private set; } // Optional FK to EventInstance
        public Guid? RedemptionId { get; private set; } // Optional FK to RedemptionRequest
        public virtual UserAccount? UserAccount { get; private set; }
        public virtual Event.EventInstance? EventInstance { get; private set; }
        public virtual Redemption.RedemptionRequest? RedemptionRequest { get; private set; }

        protected RewardTransaction() { }

        public RewardTransaction(Guid userId, int pointsDelta, string notes, TransactionType transactionType, Guid? eventId = null, Guid? redemptionId = null)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (pointsDelta == 0) throw new ArgumentException("PointsDelta cannot be zero.", nameof(pointsDelta));
            if (string.IsNullOrWhiteSpace(notes)) throw new ArgumentException("Notes cannot be empty.", nameof(notes));

            Id = Guid.NewGuid();
            UserId = userId;
            PointsDelta = pointsDelta;
            Notes = notes;
            TransactionType = transactionType;
            EventId = eventId;
            RedemptionId = redemptionId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}


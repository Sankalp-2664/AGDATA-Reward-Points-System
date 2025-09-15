using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class PointsTransaction
    {
        public int TransactionId { get; private set; }
        public int UserId { get; private set; }
        public int Points { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime TransactionDate { get; private set; }

        public PointsTransaction(int transactionId, int userId, int points, TransactionType type)
        {
            TransactionId = transactionId;
            UserId = userId;
            Points = points;
            Type = type;
            TransactionDate = DateTime.UtcNow;
        }

        public void LogTransaction()
        {
            Console.WriteLine($"Transaction: User {UserId}, Points: {Points}, Type: {Type}, Date: {TransactionDate}");
        }
    }
}

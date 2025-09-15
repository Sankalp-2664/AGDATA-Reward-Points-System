using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Admin : User
    {
        public Admin(int userId, string firstName, string lastName, string email)
            : base(userId, firstName, lastName, email, UserType.Admin) { }

        public Event CreateEvent(int eventId, string name, string description, int pointsReward, DateTime startDate, DateTime endDate)
        {
            return new Event(eventId, name, description, pointsReward, startDate, endDate);
        }

        public void AssignPoints(User user, int points)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (points <= 0) throw new ArgumentException("Points must be positive");
            user.ParticipateInEvent(new Event(0, "Manual Points Assignment", "", points, DateTime.Now, DateTime.Now));
        }

        public void ManageProducts(Product product, int newStock)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            product.IncreaseStock(newStock);
        }

        public void ViewAllTransactions(IEnumerable<PointsTransaction> transactions)
        {
            foreach (var t in transactions)
            {
                Console.WriteLine($"User: {t.UserId}, Points: {t.Points}, Type: {t.Type}, Date: {t.TransactionDate}");
            }
        }
    }
}

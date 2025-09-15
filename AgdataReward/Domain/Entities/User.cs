using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public int PointsBalance { get; private set; }
        public UserType UserType { get; private set; }

        protected User() { } // EF Core

        public User(int userId, string firstName, string lastName, string email, UserType userType)
        {
            UserId = userId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PointsBalance = 0;
            UserType = userType;
        }

        public int ViewPoints() => PointsBalance;

        public void ParticipateInEvent(Event eventItem)
        {
            if (eventItem == null) throw new ArgumentNullException(nameof(eventItem));
            PointsBalance += eventItem.PointsReward; // Points earned for participation
        }

        public void RedeemPoints(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (PointsBalance < product.PointsRequired)
                throw new InvalidOperationException("Insufficient points");
            if (product.Stock <= 0) throw new InvalidOperationException("Product out of stock");

            PointsBalance -= product.PointsRequired;
            product.DeductStock();
        }
    }

}

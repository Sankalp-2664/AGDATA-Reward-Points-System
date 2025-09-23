using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Redemption
    {
        public int RedemptionId { get; private set; }
        public int UserId { get; private set; }
        public int ProductId { get; private set; }
        public DateTime RedemptionDate { get; private set; }

        public Redemption(int redemptionId, int userId, int productId)
        {
            RedemptionId = redemptionId;
            UserId = userId;
            ProductId = productId;
            RedemptionDate = DateTime.UtcNow;
        }

        public void ProcessRedemption() => RedemptionDate = DateTime.UtcNow;

        public bool ValidatePoints(UserProfile user, Product product)
        {
            return user.PointsBalance >= product.PointsRequired && product.CheckAvailability();
        }
    }
}

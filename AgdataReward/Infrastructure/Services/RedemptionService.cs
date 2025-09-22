using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class RedemptionService : IRedemptionService
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly List<Redemption> _redemptions = new();
        private int _nextId = 1;

        public RedemptionService(IUserService userService, IProductService productService)
        {
            _userService = userService;
            _productService = productService;
        }

        public Redemption Redeem(int userId, int productId)
        {
            var user = _userService.GetUserById(userId)
                ?? throw new InvalidOperationException("User not found.");
            var product = _productService.GetById(productId)
                ?? throw new InvalidOperationException("Product not found.");

            if (user.ViewPoints() < product.PointsRequired)
                throw new InvalidOperationException("Insufficient points.");
            if (!product.CheckAvailability())
                throw new InvalidOperationException("Product out of stock.");

            // Use User entity's RedeemPoints() to deduct points & stock
            user.RedeemPoints(product);

            var redemption = new Redemption(_nextId++, userId, productId);
            _redemptions.Add(redemption);
            return redemption;
        }

        public IEnumerable<Redemption> GetUserRedemptions(int userId) =>
            _redemptions.Where(r => r.UserId == userId);
    }
}

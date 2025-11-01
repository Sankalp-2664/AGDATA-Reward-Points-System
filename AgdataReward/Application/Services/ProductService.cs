using Application.Interfaces;
using Domain.ValueObjects;
using Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRewardPointsRepository _rewardPointsRepository;

        public ProductService(IProductRepository productRepository, IRewardPointsRepository rewardPointsRepository)
        {
            _productRepository = productRepository;
            _rewardPointsRepository = rewardPointsRepository;
        }

        public async Task<ProductInformation> AddProductAsync(string skuString, string name, Guid rewardPointsId)
        {
            // Convert string to SKU value object
            var sku = new SKU(skuString);

            var rewardPoints = await _rewardPointsRepository.GetByIdAsync(rewardPointsId);
            if (rewardPoints == null)
                throw new ArgumentException("Invalid reward points configuration.");

            var product = new ProductInformation(Guid.NewGuid(), sku, name, rewardPointsId);
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<IEnumerable<ProductInformation>> GetCatalogAsync()
            => await _productRepository.ListAsync();
    }
}

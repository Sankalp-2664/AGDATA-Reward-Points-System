using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<ProductInfo> AddProductAsync(string sku, string name, Guid rewardPointsId)
        {
            var rewardPoints = await _rewardPointsRepository.GetByIdAsync(rewardPointsId);
            if (rewardPoints == null)
                throw new ArgumentException("Invalid reward points configuration.");

            var product = new ProductInfo(Guid.NewGuid(), sku, name, rewardPointsId);
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<IEnumerable<ProductInfo>> GetCatalogAsync()
        => await _productRepository.ListAsync();
    }
}

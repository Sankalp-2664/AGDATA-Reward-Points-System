using Api.Server.DTOs.Product;
using Application.Interfaces;
using AutoMapper;

namespace Api.Server.Services;

/// <summary>
/// API-facing product service that handles mapping and delegates to the
/// domain product service. Controllers stay thin and only work with DTOs.
/// </summary>
public interface IProductApiService
{
    Task<ProductInformationDto> CreateProductAsync(
        ProductInformationCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductInformationDto>> GetAllProductsAsync(
        CancellationToken cancellationToken = default);

    Task<ProductInformationDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProductAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

public class ProductApiService(
    IProductService productService,
    IMapper mapper) : IProductApiService
{
    private readonly IProductService _productService = productService;
    private readonly IMapper _mapper = mapper;

    public async Task<ProductInformationDto> CreateProductAsync(
        ProductInformationCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // Create product via domain service
        var product = await _productService.AddProductAsync(
            dto.SKU,
            dto.Name,
            dto.RewardPointsId);

        // Reload with RewardPoints included so RewardPointsValue is available
        var fullProduct = await _productService.GetByIdAsync(product.Id)
                          ?? product;

        return _mapper.Map<ProductInformationDto>(fullProduct);
    }

    public async Task<IEnumerable<ProductInformationDto>> GetAllProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetCatalogAsync();
        return _mapper.Map<IEnumerable<ProductInformationDto>>(products);
    }

    public async Task<ProductInformationDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetByIdAsync(id);
        return product is null
            ? null
            : _mapper.Map<ProductInformationDto>(product);
    }

    public async Task<bool> DeleteProductAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _productService.DeleteProductAsync(id);
    }
}

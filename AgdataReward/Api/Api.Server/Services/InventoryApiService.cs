using Api.Server.DTOs.Product;
using Application.Interfaces;
using AutoMapper;

namespace Api.Server.Services;

/// <summary>
/// API-facing inventory service that handles mapping and delegates to
/// the domain inventory service.
/// </summary>
public interface IInventoryApiService
{
    Task<ProductInventoryDto?> GetInventoryAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task UpdateStockAsync(
        Guid productId,
        int quantityChange,
        CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(
        Guid productId,
        bool isActive,
        CancellationToken cancellationToken = default);
}

public class InventoryApiService(
    IInventoryService inventoryService,
    IMapper mapper) : IInventoryApiService
{
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IMapper _mapper = mapper;

    public async Task<ProductInventoryDto?> GetInventoryAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryService.GetInventoryAsync(productId);
        return inventory is null
            ? null
            : _mapper.Map<ProductInventoryDto>(inventory);
    }

    public async Task UpdateStockAsync(
        Guid productId,
        int quantityChange,
        CancellationToken cancellationToken = default)
    {
        await _inventoryService.UpdateStockAsync(productId, quantityChange);
    }

    public async Task UpdateStatusAsync(
        Guid productId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        await _inventoryService.UpdateStatusAsync(productId, isActive);
    }
}

using Api.Server.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IMapper _mapper;

    public InventoryController(IInventoryService inventoryService, IMapper mapper)
    {
        _inventoryService = inventoryService;
        _mapper = mapper;
    }

    // GET INVENTORY BY PRODUCT ID
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetInventory(Guid productId)
    {
        var inventory = await _inventoryService.GetInventoryAsync(productId);
        if (inventory == null) return NotFound();
        var result = _mapper.Map<ProductInventoryDto>(inventory);
        return Ok(result);
    }

    // UPDATE STOCK
    public class UpdateStockRequest
    {
        public int QuantityChange { get; set; }
    }

    [HttpPost("{productId:guid}/update-stock")]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateStockRequest request)
    {
        if (request.QuantityChange == 0)
            return BadRequest("QuantityChange cannot be zero.");

        await _inventoryService.UpdateStockAsync(productId, request.QuantityChange);
        return Ok(new { message = "Stock updated successfully." });
    }
}

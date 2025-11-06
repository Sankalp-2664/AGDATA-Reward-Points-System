using Api.Server.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IInventoryService _inventoryService;

    public ProductController(IProductService productService, IInventoryService inventoryService)
    {
        _productService = productService;
        _inventoryService = inventoryService;
    }

    // POST: api/product
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto dto)
    {
        var product = await _productService.AddProductAsync(dto.Sku, dto.Name, dto.RewardPointsId);

        var resultDto = new ProductDto
        {
            ProductId = product.Id,
            Sku = product.SKU.Value,  
            Name = product.Name,
            RewardPointsId = product.RewardPointsId,
            Stock = 0 
        };

        return Ok(resultDto);
    }

    // PUT: api/product/{productId}/stock
    [HttpPut("{productId:guid}/stock")]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromQuery] int change)
    {
        await _inventoryService.UpdateStockAsync(productId, change);
        return Ok();
    }
}

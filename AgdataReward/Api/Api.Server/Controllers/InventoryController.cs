using Api.Server.DTOs.Product;
using Api.Server.Services;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

/// <summary>
/// Manages product inventory operations inside the reward system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize] // Temporarily disabled for testing - Re-enable in production
public class InventoryController(IInventoryApiService inventoryApiService) : ControllerBase
{
    private readonly IInventoryApiService _inventoryApiService = inventoryApiService;

    /// <summary>
    /// Retrieves inventory information for a specific product.
    /// </summary>
    /// <param name="productId">Unique identifier of the product.</param>
    /// <returns>Product inventory details.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/inventory/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///       "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "stockQuantity": 10,
    ///       "isActive": true
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns inventory details for the product.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If no inventory record exists for the given product.</response>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ProductInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInventory(Guid productId)
    {
        var result = await _inventoryApiService.GetInventoryAsync(productId);
        if (result is null)
            return NotFound(new { message = "Inventory record not found." });

        return Ok(result);
    }

    /// <summary>
    /// Payload used for updating stock values.
    /// </summary>
    public class UpdateStockRequest
    {
        /// <summary>
        /// The quantity to adjust.  
        /// Positive values increase stock, negative values decrease stock.
        /// </summary>
        public int QuantityChange { get; set; }
    }

    /// <summary>
    /// Updates the stock quantity for a product.
    /// </summary>
    /// <param name="productId">Product identifier.</param>
    /// <param name="request">The stock update request payload.</param>
    /// <returns>Status of the update action.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/inventory/{productId}/update-stock
    ///     {
    ///       "quantityChange": 5
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Stock updated successfully.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have admin permissions.</response>
    [HttpPost("{productId:guid}/update-stock")]
    [AllowAnonymous] // Temporarily allow for testing - Re-enable [Authorize(Roles = "Admin")] in production
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateStockRequest request)
    {
        if (request.QuantityChange == 0)
            return BadRequest(new { message = "Quantity change must be non-zero." });

        await _inventoryApiService.UpdateStockAsync(productId, request.QuantityChange);

        return Ok(new { message = "Stock updated successfully." });
    }

    /// <summary>
    /// Payload used for updating product status.
    /// </summary>
    public class InventoryUpdateStatusRequest
    {
        /// <summary>
        /// Whether the product should be active or inactive.
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Updates the active status for a product.
    /// </summary>
    /// <param name="productId">Product identifier.</param>
    /// <param name="request">The status update request payload.</param>
    /// <returns>Status of the update action.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/inventory/{productId}/update-status
    ///     {
    ///       "isActive": true
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Status updated successfully.</response>
    /// <response code="400">If the request data is invalid.</response>
    [HttpPost("{productId:guid}/update-status")]
    [AllowAnonymous] // Temporarily allow for testing - Re-enable authorization in production
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(Guid productId, [FromBody] InventoryUpdateStatusRequest request)
    {
        await _inventoryApiService.UpdateStatusAsync(productId, request.IsActive);
        return Ok(new { message = "Status updated successfully." });
    }
}

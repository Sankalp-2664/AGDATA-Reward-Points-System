using Api.Server.DTOs.Product;
using Api.Server.Services;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

/// <summary>
/// Manages reward products and their metadata.
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize] // Temporarily disabled for testing - Re-enable in production
public class ProductController(IProductApiService productApiService) : ControllerBase
{
    private readonly IProductApiService _productApiService = productApiService;


    /// <summary>
    /// Creates a new product in the reward catalog.
    /// </summary>
    /// <param name="dto">Product creation payload.</param>
    /// <returns>The newly created product.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/product
    ///     {
    ///       "sku": "ITEM-001",
    ///       "name": "Coffee Mug",
    ///       "rewardPointsId": "a3b5c7d9-0000-1111-2222-333344445555"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created product.</response>
    /// <response code="400">If the payload is invalid.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="403">If the caller is not an admin.</response>
    /// <response code="409">If a product with the same SKU already exists.</response>
    [HttpPost]
    [AllowAnonymous] // Temporarily allow for testing - Re-enable [Authorize(Roles = "Admin")] in production
    [ProducesResponseType(typeof(ProductInformationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProductInformationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _productApiService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all products available in the reward catalog.
    /// </summary>
    /// <returns>List of products with associated reward points info.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/product
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     [
    ///       {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "sku": "ITEM-001",
    ///         "name": "Coffee Mug",
    ///         "rewardPointsId": "a3b5c7d9-0000-1111-2222-333344445555",
    ///         "rewardPointsValue": 100
    ///       }
    ///     ]
    /// </remarks>
    /// <response code="200">Returns the list of products.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    [HttpGet]
    [AllowAnonymous] // Allow access for product listing
    [ProducesResponseType(typeof(IEnumerable<ProductInformationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productApiService.GetAllProductsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all products with inventory information (for admin dashboard).
    /// </summary>
    /// <returns>List of products with inventory details.</returns>
    /// <response code="200">Returns the list of products with inventory.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    [HttpGet("with-inventory")]
    [AllowAnonymous] // Allow access for admin dashboard
    [ProducesResponseType(typeof(IEnumerable<ProductWithInventoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllWithInventory()
    {
        var result = await _productApiService.GetAllProductsWithInventoryAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single product by its identifier.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>The product with reward points info.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/product/{id}
    ///
    /// </remarks>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductInformationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productApiService.GetProductByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Deletes a product from the reward catalog.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>Status of the delete operation.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/product/{id}
    ///
    /// </remarks>
    /// <response code="200">Product deleted successfully.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="403">If the caller is not an admin.</response>
    /// <response code="404">If the product was not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")] // only Admin can delete products
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productApiService.DeleteProductAsync(id);

        if (!deleted)
            return NotFound(new { message = "Product not found." });

        return Ok(new { message = "Product deleted successfully." });
    }

    /// <summary>
    /// Updates a product's information.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="dto">Updated product information.</param>
    /// <returns>The updated product with inventory.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/product/{id}
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "sku": "ITEM-001-UPDATED",
    ///       "name": "Updated Product Name",
    ///       "rewardPointsId": "a3b5c7d9-0000-1111-2222-333344445555"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Product updated successfully.</response>
    /// <response code="400">If the payload is invalid.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="403">If the caller is not an admin.</response>
    /// <response code="404">If the product was not found.</response>
    [HttpPut("{id:guid}")]
    [AllowAnonymous] // Temporarily allow for testing - Re-enable [Authorize(Roles = "Admin")] in production
    [ProducesResponseType(typeof(ProductWithInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductInformationUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Id != id)
            return BadRequest(new { message = "ID mismatch between URL and payload." });

        try
        {
            var result = await _productApiService.UpdateProductAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

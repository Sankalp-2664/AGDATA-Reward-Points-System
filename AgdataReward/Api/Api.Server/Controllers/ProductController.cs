using Api.Server.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IRewardPointsRepository _rewardPointsRepo;

    public ProductController(IProductService productService, IMapper mapper, IRewardPointsRepository rewardPointsRepo)
    {
        _productService = productService;
        _mapper = mapper;
        _rewardPointsRepo = rewardPointsRepo;
    }

    // CREATE PRODUCT
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductInformationCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var product = await _productService.AddProductAsync(dto.SKU, dto.Name, dto.RewardPointsId);

            // reload for rewardPointsValue
            var fullProduct = await _productService.GetByIdAsync(product.Id);

            var result = _mapper.Map<ProductInformationDto>(fullProduct);
            var rp = await _rewardPointsRepo.GetByIdAsync(result.RewardPointsId);
            result.RewardPointsValue = rp?.PointsValue ?? 0;
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

    // GET ALL PRODUCTS
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetCatalogAsync();
        var result = new List<ProductInformationDto>();

        foreach (var product in products)
        {
            var dto = _mapper.Map<ProductInformationDto>(product);

            var rp = await _rewardPointsRepo.GetByIdAsync(dto.RewardPointsId);
            dto.RewardPointsValue = rp?.PointsValue ?? 0;

            result.Add(dto);
        }

        return Ok(result);
    }

    // GET PRODUCT BY ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        var result = _mapper.Map<ProductInformationDto>(product);

        var rp = await _rewardPointsRepo.GetByIdAsync(result.RewardPointsId);
        result.RewardPointsValue = rp?.PointsValue ?? 0;

        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteProductAsync(id);

        if (!deleted)
            return NotFound(new { message = "Product not found." });

        return Ok(new { message = "Product deleted successfully." });
    }

}

using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedemptionController : ControllerBase
    {
        private readonly IRedemptionService _redemptionService;

        public RedemptionController(IRedemptionService redemptionService)
        {
            _redemptionService = redemptionService;
        }

        // POST: api/redemption/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestRedemption([FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            var redemption = await _redemptionService.RequestRedemptionAsync(userId, productId);
            return Ok(redemption);
        }

        // PUT: api/redemption/{id}/approve
        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _redemptionService.ApproveRedemptionAsync(id);
            return Ok();
        }

        // PUT: api/redemption/{id}/complete
        [HttpPut("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _redemptionService.CompleteRedemptionAsync(id);
            return Ok();
        }
    }
}

using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/transaction/user/{userId}
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserTransactions(Guid userId)
        {
            var tx = await _transactionService.GetUserTransactionsAsync(userId);
            return Ok(tx);
        }
    }
}

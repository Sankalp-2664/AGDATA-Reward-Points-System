using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRewardTransactionRepository _transactionRepo;

        public TransactionService(IRewardTransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId)
            => await _transactionRepo.GetByUserIdAsync(userId);
    }
}

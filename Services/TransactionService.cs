using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.Validation_and_Enums;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class TransactionService : ITransactionService
    {
        private IMongoRepository _repository;
        private ILogger<TransactionService> _logger;
        public TransactionService(IMongoRepository repository, ILogger<TransactionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<bool> AddTransection(string TranxId, string Name, double Amount, StatusEnum Status, LiveAnimal liveAnimal, Order order)
        {
            Transaction transaction = new Transaction
            {
                TranxId = TranxId,
                Name = Name,
                Amount = Amount,
                Status = Status,
            };
            if (liveAnimal != null) transaction.Product = liveAnimal;
            if (order != null) transaction.Order = order;
            await _repository.SaveAsync<Transaction>(transaction);
            return true;
        }

    }
}

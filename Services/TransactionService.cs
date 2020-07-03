using System;
using System.Collections.Generic;
using System.Linq;
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
            var id = Guid.NewGuid().ToString();
            transaction.Id = id;
            if (liveAnimal != null) transaction.Product = liveAnimal;
            if (order != null) transaction.Order = order;
            transaction.transactionTime = DateTime.Now;
            await _repository.SaveAsync<Transaction>(transaction);
            return true;
        }
        public async Task<List<Transaction>> GetAllTransaction()
        {
            try
            {
                var transctions = await _repository.GetItemsAsync<Transaction>(e=>e.Status == StatusEnum.Success);
                var transactionssrRes = transctions?.ToList();
                transactionssrRes?.Reverse();
                return transactionssrRes;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllTransactionList Failed: {e.Message}");
                return null;
            }

        }
        public async Task<List<Transaction>> GetAllFailureTransaction()
        {
            try
            {
                var transctions = await _repository.GetItemsAsync<Transaction>(e => e.Status == StatusEnum.Failure);
                var transactionssrRes = transctions?.ToList();
                transactionssrRes?.Reverse();
                return transactionssrRes;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllTransactionList Failed: {e.Message}");
                return null;
            }

        }
        public async Task<Transaction> GetTransactionByOrderId(string OrderId)
        {
            try
            {
                var transction = await _repository.GetItemAsync<Transaction>(e => e.Order.Id == OrderId);
                
                return transction;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllTransactionList Failed: {e.Message}");
                return null;
            }

        }

    }
}

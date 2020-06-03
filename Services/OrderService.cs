using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IMongoRepository mongoRepository)
        {
            _repository = mongoRepository;
        }
        public async Task<bool> AddOrder(Order order)
        {
            try
            {
                await _repository.SaveAsync<Order>(order);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddOrder Failed: {e.Message}");
                return false;
            }
        }
    }
}

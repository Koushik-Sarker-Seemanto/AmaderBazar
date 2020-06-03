using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.OrderModels;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<OrderService> _logger;
        private readonly ILiveAnimalService _liveAnimalService;
        public OrderService(ILogger<OrderService> logger,IMongoRepository mongoRepository,ILiveAnimalService liveAnimalService)
        {
            _repository = mongoRepository;
            _logger = logger;
            _liveAnimalService = liveAnimalService;
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

        public async Task<List<OrderViewModel>> PlacedOrders()
        {
            try
            {
                var orders = await _repository.GetItemsAsync<Order>();
                var list = orders?.ToList();
                list.Reverse();
                List<OrderViewModel> orderViewModels = new List<OrderViewModel>();
                foreach (var order in orders)
                {
                    var orderView = await BuildOrderViewModel(order);
                    orderViewModels.Add(orderView);
                }

                return orderViewModels;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetOrderList Failed: {e.Message}");
                return null;
            }
        }

        public async Task<bool> ContactClient(string OrderId)
        {
            try
            {
                var order = await _repository.GetItemAsync<Order>(e => e.Id == OrderId);
                if (order == null)
                {
                    return false;
                }
                if (order.Contacted)
                {
                    _logger.LogInformation($"ContactClient: Already Contacted.");
                    return false;
                }
                order.Contacted = true;
                await _repository.UpdateAsync<Order>(e => e.Id == order.Id, order);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAnimalDetails Failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOrder(string OrderId)
        {
            try
            {
                await _repository.DeleteAsync<Order>(e => e.Id == OrderId);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Delete Animal Failed: {e.Message}");
                return false;
            }
        }


        private async Task<OrderViewModel> BuildOrderViewModel(Order order)
        {
            var liveAnimal = await _liveAnimalService.GetLiveAnimalById(order.LiveAnimalId);
            OrderViewModel orderView = new OrderViewModel
            {
                Order = order,
                LiveAnimal = liveAnimal,

            };
            return orderView;
        }
    }
}

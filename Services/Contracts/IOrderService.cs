using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.OrderModels;

namespace Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> AddOrder(Order order);
        Task<List<OrderViewModel>> PlacedOrders();
        Task<bool> ContactClient(string OrderId);
        Task<bool> DeleteOrder(string OrderId);


    }
}

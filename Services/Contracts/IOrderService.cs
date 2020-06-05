using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.AdminModels;
using Models.Entities;
using Models.LiveAnimalModels;
using Models.OrderModels;

namespace Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> AddOrder(Order order);
        Task<Order> FindOrderById(string id);
        Task<List<OrderViewModel>> PlacedOrders();
        Task<bool> ContactClient(string OrderId);
        Task<bool> DeleteOrder(string OrderId);
        public FileStreamResult CreateReciept(OrderViewModel orderDetailes);


    }
}

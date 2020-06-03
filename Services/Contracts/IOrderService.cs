using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace Services.Contracts
{
    public interface IOrderService
    {
        Task<bool> AddOrder(Order order);
    }
}

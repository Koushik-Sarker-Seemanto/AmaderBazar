using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.OrderModels;

namespace Services.Contracts
{
    public interface IPaymentService
    {
        public Task<bool> ValidatePaymentRequest(IFormCollection request);
        public FileStreamResult CreateReciept(OrderViewModel model, Transaction transaction);
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts
{
    public interface IPaymentService
    {
        public Task<bool> ValidatePaymentRequest(IFormCollection request);
    }
}
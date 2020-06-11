using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Newtonsoft.Json;
using Repositories;
using Services.Contracts;
using SSLCommerz.Contracts;

namespace Services
{
    public class PaymentService: IPaymentService
    {
        private IMongoRepository _repository;
        private ILogger<PaymentService> _logger;
        private ISSLCommerzService _sslCommerzService;
        public PaymentService(IMongoRepository repository, ILogger<PaymentService> logger, ISSLCommerzService sslCommerzService)
        {
            _repository = repository;
            _logger = logger;
            _sslCommerzService = sslCommerzService;
        }
        
        public async Task<bool> ValidatePaymentRequest(IFormCollection request)
        {
            _logger.LogInformation($"Request.Form: {JsonConvert.SerializeObject(request)}");
            string trxId = request["tran_id"];
            string valId = request["val_id"];
                
            // AMOUNT and Currency FROM DB FOR THIS TRANSACTION
            string amount = "";
            if (!string.IsNullOrEmpty(trxId))
            {
                var orderData = await _repository.GetItemAsync<Order>(e=>e.Id == trxId);
                if (orderData == null)
                {
                    _logger.LogInformation("OderData is null for this Trxid");
                    return false;
                }
                var animal = await _repository.GetItemAsync<LiveAnimal>(e => e.Id == orderData.LiveAnimalId);
                if (animal == null)
                {
                    _logger.LogInformation("Live Animal is null for this Trxid");
                    return false;
                }
                amount = animal.Price.ToString();
            }
            NameValueCollection collection = new NameValueCollection();
            foreach (var pair in request)
            {
                collection.Add(pair.Key, pair.Value);
            }

                
            _logger.LogInformation($"NameValueCollection: {JsonConvert.SerializeObject(collection)}");

            var validate = _sslCommerzService.OrderValidate(collection, valId, trxId, amount, "BDT");
            if (validate)
            {
                return true;
            }
            else
            {
                _logger.LogInformation($"validate = _sslCommerzService.OrderValidate: false");
                return false;
            }
        }
    }
}
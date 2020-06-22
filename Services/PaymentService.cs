using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.Validation_and_Enums;
using Newtonsoft.Json;
using Repositories;
using Services.Contracts;
using SSLCommerz.Contracts;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private IMongoRepository _repository;
        private ILogger<PaymentService> _logger;
        private ISSLCommerzService _sslCommerzService;
        private readonly ITransactionService _transactionService;
        private readonly IAdminPanelServices _adminPanelServices;

        public PaymentService(IMongoRepository repository, IAdminPanelServices adminPanelService,
            ITransactionService transactionService, ILogger<PaymentService> logger,
            ISSLCommerzService sslCommerzService)
        {
            _repository = repository;
            _logger = logger;
            _sslCommerzService = sslCommerzService;
            _adminPanelServices = adminPanelService;
            _transactionService = transactionService;
        }

        public async Task<bool> ValidatePaymentRequest(IFormCollection request)
        {
            _logger.LogInformation($"Request.Form: {JsonConvert.SerializeObject(request)}");
            string trxId = ""+request["tran_id"];
            string valId = "" + request["val_id"];
            string _amount = "" + request["amount"].ToString();
            double Amount = ConvertToDouble(_amount);
            
            string  Name = "" + request["cus_name"];

            // AMOUNT and Currency FROM DB FOR THIS TRANSACTION
            string amount = "";
            if (!string.IsNullOrEmpty(trxId))
            {
                var orderData = await _repository.GetItemAsync<Order>(e => e.Id == trxId);
                if (orderData == null)
                {
                    await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, null,null);
                    _logger.LogInformation("OderData is null for this Trxid");
                    return false;
                }

                var animal = await _repository.GetItemAsync<LiveAnimal>(e => e.Id == orderData.LiveAnimalId);
                if (animal == null)
                {
                    await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, null, orderData);
                    _logger.LogInformation("Live Animal is null for this Trxid");
                    return false;
                }

                amount = animal.Price.ToString();

                NameValueCollection collection = new NameValueCollection();
                foreach (var pair in request)
                {
                    collection.Add(pair.Key, pair.Value);
                }


                _logger.LogInformation($"NameValueCollection: {JsonConvert.SerializeObject(collection)}");

                var validate = _sslCommerzService.OrderValidate(collection, valId, trxId, amount, "BDT");
                if (validate)
                {
                    var res = await _adminPanelServices.SellAnimal(animal.Id);
                    if (res)
                    {

                        var result = await _transactionService.AddTransection(trxId,Name,Amount,StatusEnum.Success,animal,orderData);
                        return result;
                    }
                    var resultF = await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Success, animal, orderData);
                    return res;
                }
                else
                {
                    _logger.LogInformation($"validate = _sslCommerzService.OrderValidate: false");
                    await _transactionService.AddTransection(trxId, Name, Amount, StatusEnum.Failure, animal, orderData);
                    return false;
                }
            }
            return false;
        }
        private double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

    }
}
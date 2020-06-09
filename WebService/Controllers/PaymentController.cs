using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.LiveAnimalModels;
using Models.OrderModels;
using Newtonsoft.Json;
using Services.Contracts;
using SSLCommerz.Contracts;
using SSLCommerz.Models;

namespace WebService.Controllers
{
    public class PaymentController : Controller
    {
        private ILogger<PaymentController> _logger;
        private IOrderService _orderService;
        private ILiveAnimalService _liveAnimalService;
        private ISSLCommerzService _sslCommerzService;
        public PaymentController(ILogger<PaymentController> logger, IOrderService orderService, ILiveAnimalService liveAnimalService, ISSLCommerzService sslCommerzService)
        {
            _logger = logger;
            _orderService = orderService;
            _liveAnimalService = liveAnimalService;
            _sslCommerzService = sslCommerzService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind] SearchOrderModel order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"!ModelState.IsValid");
                return View(order);
            }

            if (string.IsNullOrEmpty(order.Id))
            {
                _logger.LogInformation($"string.IsNullOrEmpty(order.Id)");
                ModelState.AddModelError("", "OrderId can't be null or empty!!!");
                return View(order);
            }
            var orderData = await _orderService.FindOrderById(order.Id);
            if (orderData == null)
            {
                ModelState.AddModelError("", "OrderId Not found");
                return View(order);
            }
            _logger.LogInformation($"RedirectToAction: {order.Id}");

            return RedirectToAction("OrderDetails", "Payment", new { Id = order.Id});
        }
        
        public async Task<IActionResult> OrderDetails(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("Index", "Payment");
            }
            
            var orderData = await _orderService.FindOrderById(Id);
            if (orderData == null)
            {
                ModelState.AddModelError("", "OrderId Not found");
                return RedirectToAction("Index", "Payment");
            }
            var liveAnimal = await _liveAnimalService.GetLiveAnimalById(orderData.LiveAnimalId);
            if (liveAnimal == null)
            {
                ModelState.AddModelError("", "Ordered Animal is missing.");
                return RedirectToAction("Index", "Payment");
            }
            LiveAnimalDetailsViewModel viewModel = new LiveAnimalDetailsViewModel
            {
                Order = orderData,
                Related = null,
                LiveAnimalDetails = liveAnimal,
            };
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDetails([Bind] LiveAnimalDetailsViewModel model)
        {
            NameValueCollection PostData = new NameValueCollection();
            
            //Basic Infos.
            PostData.Add("total_amount", model.LiveAnimalDetails.Price.ToString());
            PostData.Add("tran_id", model.Order.Id);
            PostData.Add("success_url", "http://localhost:5000/payment/success");
            PostData.Add("fail_url", "http://localhost:5000/payment/fail");
            PostData.Add("cancel_url", "http://localhost:5000/payment/cancel");
            
            //Customer Info.
            PostData.Add("cus_name", model.Order.Name);
            PostData.Add( "cus_add1", model.Order.Address);
            PostData.Add("cus_phone", model.Order.PhoneNumber);
            
            //Product Infos.
            PostData.Add("product_name", model.LiveAnimalDetails.Id);
            PostData.Add("product_category",model.LiveAnimalDetails.Title);
            
            _logger.LogInformation($"SSL COmerzzzzzzzzzzzzzzz NmaeValueCollection: {JsonConvert.SerializeObject(PostData)}");
            
            SSLCommerzInitResponse response = _sslCommerzService.InitiateTransaction(PostData);
            _logger.LogInformation($"SSL COmerzzzzzzzzzzzzzzz Responseeeeeee: {JsonConvert.SerializeObject(response)}");
            return Redirect(response.GatewayPageURL);
        }
        
        public async Task<IActionResult> Success()
        {
            if (!String.IsNullOrEmpty(Request.Form["status"]) && Request.Form["status"] == "VALID")
            {
                _logger.LogInformation($"Request.Form: {JsonConvert.SerializeObject(Request.Form)}");
                string TrxID = Request.Form["tran_id"];
                string valId = Request.Form["val_id"];
                
                // AMOUNT and Currency FROM DB FOR THIS TRANSACTION
                string amount = "";
                if (!string.IsNullOrEmpty(TrxID))
                {
                    var orderData = await _orderService.FindOrderById(TrxID);
                    if (orderData == null)
                    {
                        _logger.LogInformation("OderData is null for this Trxid");
                        return RedirectToAction("Index", "Payment");
                    }
                    var liveAnimal = await _liveAnimalService.GetLiveAnimalById(orderData.LiveAnimalId);
                    if (liveAnimal == null)
                    {
                        _logger.LogInformation("Live Animal is null for this Trxid");
                        return RedirectToAction("Index", "Payment");
                    }

                    amount = liveAnimal.Price.ToString();
                }
                string currency = "BDT";
                
                NameValueCollection collection = new NameValueCollection();
                foreach (var pair in Request.Form)
                {
                    collection.Add(pair.Key, pair.Value);
                }

                
                _logger.LogInformation($"NameValueCollection: {JsonConvert.SerializeObject(collection)}");

                var validate = _sslCommerzService.OrderValidate(collection, valId,TrxID, amount, currency);
                if (validate)
                {
                    return View();
                }
                else
                {
                    _logger.LogInformation($"validate = _sslCommerzService.OrderValidate: false");
                }
            }

            return RedirectToAction("Index", "Payment");
        }

        public void IPNListener()
        {
            _logger.LogInformation($"IPNListenerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
        }
    }
}

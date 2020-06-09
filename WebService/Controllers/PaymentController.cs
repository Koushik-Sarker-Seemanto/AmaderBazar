using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
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
        private IPaymentService _paymentService;
        public PaymentController(ILogger<PaymentController> logger, IOrderService orderService, ILiveAnimalService liveAnimalService, ISSLCommerzService sslCommerzService, IPaymentService paymentService)
        {
            _logger = logger;
            _orderService = orderService;
            _liveAnimalService = liveAnimalService;
            _sslCommerzService = sslCommerzService;
            _paymentService = paymentService;
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
            PostData.Add("success_url", "http://localhost:5000/Payment/PaymentCheck");
            PostData.Add("fail_url", "http://localhost:5000/Payment/PaymentCheck");
            PostData.Add("cancel_url", "http://localhost:5000/Payment/PaymentCheck");
            
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
        
        public async Task<IActionResult> PaymentCheck()
        {
            if (!String.IsNullOrEmpty(Request.Form["status"]) && Request.Form["status"] == "VALID")
            {
                var result = await _paymentService.ValidatePaymentRequest(Request.Form);
                if (result)
                {
                    _logger.LogInformation($"Success page");
                    return RedirectToAction("Success", "Payment");
                }
                else
                {
                    _logger.LogInformation($"Invalid page");
                    return RedirectToAction("InvalidPayment", "Payment");
                }
            }
            if (!String.IsNullOrEmpty(Request.Form["status"]) && Request.Form["status"] == "FAILED")
            {
                _logger.LogInformation($"Failed page");
                _logger.LogInformation($"Failed Response: {JsonConvert.SerializeObject(Request.Form)}");
                return RedirectToAction("Failed", "Payment");
            }
            if (!String.IsNullOrEmpty(Request.Form["status"]) && Request.Form["status"] == "CANCELLED")
            {
                _logger.LogInformation($"Cancel page");
                _logger.LogInformation($"Failed Response: {JsonConvert.SerializeObject(Request.Form)}");
                return RedirectToAction("Cancelled", "Payment");
            }

            return RedirectToAction("Index", "Payment");
        }

        public IActionResult Success(IFormCollection formCollection)
        {
            return View(formCollection);
        }

        public IActionResult InvalidPayment()
        {
            return View();
        }

        public IActionResult Failed()
        {
            return View();
        }

        public IActionResult Cancelled()
        {
            return View();
        }

        public void IPNListener()
        {
            _logger.LogInformation($"IPNListenerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
        }
    }
}

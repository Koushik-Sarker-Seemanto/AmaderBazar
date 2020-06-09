using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
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
            
            /*PostData.Add("value_a", "ref00");
            PostData.Add("value_b", "ref00");
            PostData.Add("value_c", "ref00");
            PostData.Add("value_d", "ref00");*/
            
            SSLCommerzInitResponse response = _sslCommerzService.InitiateTransaction(PostData);
            _logger.LogInformation($"SSL COmerzzzzzzzzzzzzzzz Responseeeeeee: {JsonConvert.SerializeObject(response)}");
            return Redirect(response.GatewayPageURL);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

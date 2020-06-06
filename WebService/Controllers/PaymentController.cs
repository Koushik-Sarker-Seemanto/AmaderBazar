using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.LiveAnimalModels;
using Models.OrderModels;
using Services.Contracts;

namespace WebService.Controllers
{
    public class PaymentController : Controller
    {
        private ILogger<PaymentController> _logger;
        private IOrderService _orderService;
        private ILiveAnimalService _liveAnimalService;
        public PaymentController(ILogger<PaymentController> logger, IOrderService orderService, ILiveAnimalService liveAnimalService)
        {
            _logger = logger;
            _orderService = orderService;
            _liveAnimalService = liveAnimalService;
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
    }
}
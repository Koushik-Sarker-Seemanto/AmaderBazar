using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.LiveAnimalModels;
using Services.Contracts;

namespace WebService.Controllers
{
    public class LiveAnimalController : Controller
    {
        private readonly ILiveAnimalService _liveAnimalService;
        private readonly IOrderService _orderService;
        public LiveAnimalController(ILiveAnimalService liveAnimalService, IOrderService orderService)
        {
            _liveAnimalService = liveAnimalService;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            var animals = await _liveAnimalService.GetAllLiveAnimals();
            return View(animals);
        }

        public async Task<IActionResult> Details(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("Index", "LiveAnimal");
            }

            LiveAnimalDetailsViewModel viewModel = new LiveAnimalDetailsViewModel();
            
            var liveAnimalDetails = await _liveAnimalService.GetLiveAnimalById(Id);
            viewModel.LiveAnimalDetails = liveAnimalDetails;
            viewModel.Related = await GetRelated(liveAnimalDetails.Category);
            return View(viewModel);
        }

        public async Task<IActionResult> Order(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("Index", "LiveAnimal");
            }
            
            LiveAnimalDetailsViewModel viewModel = new LiveAnimalDetailsViewModel();
            
            var liveAnimalDetails = await _liveAnimalService.GetLiveAnimalById(Id);
            viewModel.LiveAnimalDetails = liveAnimalDetails;
            viewModel.Related = await GetRelated(liveAnimalDetails.Category);
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order([Bind] Order order)
        {
            LiveAnimalDetailsViewModel viewModel = new LiveAnimalDetailsViewModel();
            var liveAnimalDetails = await _liveAnimalService.GetLiveAnimalById(order.LiveAnimalId);
            viewModel.LiveAnimalDetails = liveAnimalDetails;
            viewModel.Related = await GetRelated(liveAnimalDetails.Category);
            viewModel.Order = order;
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }
            var id = Guid.NewGuid().ToString();
            order.Id = id;
            await _orderService.AddOrder(order);
            return RedirectToAction("Index", "LiveAnimal");
        }

        private async Task<List<LiveAnimalViewModelFrontend>> GetRelated(string category)
        {
            var related = await _liveAnimalService.GetLiveAnimalByCategory(category);
            return related.Take(9).ToList();
        }
    }
}

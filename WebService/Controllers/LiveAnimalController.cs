using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.LiveAnimalsModels;
using Services.Contracts;

namespace WebService.Controllers
{
    public class LiveAnimalController : Controller
    {
        private readonly ILiveAnimalService _liveAnimalService;
        public LiveAnimalController(ILiveAnimalService liveAnimalService)
        {
            _liveAnimalService = liveAnimalService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.LiveAnimals = await _liveAnimalService.GetAllLiveAnimals();
            return View();
        }

        public async Task<IActionResult> Details(string Id)
        {
            
            var LiveAnimalDetails = await _liveAnimalService.GetLiveAnimalById(Id);
            ViewBag.LiveAnimalDetails = LiveAnimalDetails;
            ViewBag.Related = await GetRelated(LiveAnimalDetails.Category);
            return View();
        }

        public async Task<IActionResult> Order(string Id)
        {
            
            var LiveAnimalDetails = await _liveAnimalService.GetLiveAnimalById(Id);
            ViewBag.Related = await GetRelated(LiveAnimalDetails.Category);
            ViewBag.LiveAnimalDetails = LiveAnimalDetails;
            return View();
        }

        private async Task<List<LiveAnimalViewModelFrontend>> GetRelated(string category)
        {
            var related = await _liveAnimalService.GetLiveAnimalByCategory(category);
            if (related.Count > 9) related.RemoveRange(9, related.Count - 9);
            return related;
        }
    }
}

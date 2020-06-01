using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using Models.LiveAnimalModels;
using Services.Contracts;
using WebService.Models;



namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILiveAnimalService _liveAnimalService;
        [TempData] public bool Is_English { get; set; }
        public HomeController(ILogger<HomeController> logger,ILiveAnimalService liveAnimalService)
        { 
            _logger = logger;
            _liveAnimalService = liveAnimalService;

        }
        public IActionResult AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            if (!TempData.ContainsKey("Is_English"))
            {
                TempData["Is_English"] = true;
            }
            var Featured = await _liveAnimalService.GetFeaturedLiveAnimal();
            var Latest = await _liveAnimalService.GetLatestAnimal();
            if(Featured != null && Latest != null)
            {
                IndexViewModel animals = new IndexViewModel
                {
                    Featured = Featured,
                    Latest = Latest,
                };
                return View(animals);
            }
            return RedirectToAction("Error");
        }

        public IActionResult ChangeLanguage(bool _language)
        {
            Is_English = _language;
            return RedirectToAction("Index","Home");
        }

        
        

        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

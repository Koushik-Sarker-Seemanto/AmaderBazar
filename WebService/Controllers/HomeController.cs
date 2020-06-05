using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.LiveAnimalModels;
using Services.Contracts;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using WebService.Models;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;
using RectangleF = Syncfusion.Drawing.RectangleF;
using SizeF = Syncfusion.Drawing.SizeF;


namespace WebService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILiveAnimalService _liveAnimalService;

        public HomeController(ILogger<HomeController> logger,ILiveAnimalService liveAnimalService)
        {
            _logger = logger;
            _liveAnimalService = liveAnimalService;
        }

        public async Task<IActionResult> Index()
        {
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

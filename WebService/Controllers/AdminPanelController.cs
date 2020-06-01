using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize]
    public class AdminPanelController: Controller
    {
        private readonly IAdminPanelServices _adminPanelServices;
        private readonly ILogger<AdminAuthController> _logger;

        public AdminPanelController(IAdminPanelServices adminPanelServices,
            ILogger<AdminAuthController> logger)
        {
            _adminPanelServices = adminPanelServices;
            _logger = logger;
        }
        
        public async Task<IActionResult> Index()
        {
            AdminIndexViewModel results = await _adminPanelServices.GetAnimalList();
            return View(results);
        }

        public async Task<IActionResult> AddAnimal()
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnimal([Bind] LiveAnimalViewModel model)
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            if(ModelState.IsValid == false)
            {
                return View(model);
            }

            var id = Guid.NewGuid().ToString();
            model.Id = id;
            
            _logger.LogInformation($"AddAnimal: {JsonConvert.SerializeObject(model)}");

            await _adminPanelServices.AddAnimal(model);
            
            return RedirectToAction("Index", "AdminPanel");

        }
        
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory([Bind] Category model)
        {
            if (ModelState.IsValid == false)
            {
                return RedirectToAction("Index", "AdminPanel");
            }

            var id = Guid.NewGuid().ToString();
            model.Id = id;

            _logger.LogInformation($"AddCategory: {JsonConvert.SerializeObject(model)}");
            bool result = await _adminPanelServices.AddCategory(model);
            
            return RedirectToAction("Index", "AdminPanel");
        }
        
        
    }
}
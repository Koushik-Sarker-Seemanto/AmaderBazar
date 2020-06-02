using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Newtonsoft.Json;
using Services.Contracts;
using X.PagedList;
using X.PagedList.Mvc.Core;

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
        
        public async Task<IActionResult> Index(int? page)
        {
            AdminIndexViewModel results = await _adminPanelServices.GetAnimalList();
            var list = results.LiveAnimalList.ToPagedList(page ?? 1, 10);
            return View(list);
        }

        public async Task<IActionResult> AddAnimal()
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnimal([Bind] LiveAnimalViewModel model,ICollection<IFormFile> files)
        {

            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            if(ModelState.IsValid == false )
            {
                return View(model);
            }

            var id = Guid.NewGuid().ToString();
            model.Id = id;
            var images = await _adminPanelServices.UploadImage(files);
            model.Images = images;
            
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
        
        public async Task<IActionResult> UpdateAnimal(string itemId)
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            if (string.IsNullOrEmpty(itemId))
            {
                return RedirectToAction("Index", "AdminPanel");
            }
            var result = await _adminPanelServices.GetAnimalDetails(itemId);
            LiveAnimalViewModel liveAnimalViewModel = new LiveAnimalViewModel
            {
                Id = result.Id,
                Title = result.Title,
                Category = result.Category.Id,
                Color = result.Color,
                Location = result.Location,
                Origin = result.Origin,
                Description = result.Description,
                Price = result.Price,
                Images = result.Images,
            };
            _logger.LogInformation($"AnimalInfo: {JsonConvert.SerializeObject(result)}");
            ViewBag.Images = liveAnimalViewModel.Images;
            return View(liveAnimalViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAnimal([Bind] LiveAnimalViewModel model)
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            if(ModelState.IsValid == false)
            {
                return View(model);
            }

            _logger.LogInformation($"AddAnimal: {JsonConvert.SerializeObject(model)}");

            await _adminPanelServices.UpdateAnimal(model);
            
            return RedirectToAction("AnimalDetails", "AdminPanel", new{ itemId = model.Id});
        }

        public async Task<IActionResult> SellAnimal(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return RedirectToAction("Index", "AdminPanel");
            }
            bool result = await _adminPanelServices.SellAnimal(itemId);
            
            return RedirectToAction("Index", "AdminPanel");
        }

        public async Task<IActionResult> AnimalDetails(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return RedirectToAction("Index", "AdminPanel");
            }
            var result = await _adminPanelServices.GetAnimalDetails(itemId);
            _logger.LogInformation($"AnimalInfo: {JsonConvert.SerializeObject(result)}");
            return View(result);
        }
        
        public async Task<IActionResult> AnimalDelete(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return RedirectToAction("Index", "AdminPanel");
            }

            bool result = await _adminPanelServices.DeleteAnimal(itemId);
            if (result)
            {
                return RedirectToAction("Index", "AdminPanel");
            }

            return BadRequest("Couldn't delete");
        }
    }
}
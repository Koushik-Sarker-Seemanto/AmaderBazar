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
using Models.Validation_and_Enums;
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

        private readonly IOrderService _orderServices;
        private readonly ITransactionService _transactionService;
        

        public AdminPanelController(IAdminPanelServices adminPanelServices,
            ILogger<AdminAuthController> logger,IOrderService orderService,ITransactionService
                transactionService)
        {
            _adminPanelServices = adminPanelServices;
            _orderServices = orderService;
            _logger = logger;
            _transactionService = transactionService;
        }
        
        public async Task<IActionResult> Index(int? page)
        {
            AdminIndexViewModel results = await _adminPanelServices.GetAnimalList();
            var list = results.LiveAnimalList.ToPagedList(page ?? 1, 9);
            return View(list);
        }

        public async Task<IActionResult> AddAnimal()
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnimal([Bind] LiveAnimalViewModel model, ICollection<IFormFile> files)
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
                TitleBn = result.TitleBn,
                Category = result.Category.Id,
                Color = result.Color,
                Location = result.Location,
                LocationBn = result.LocationBn,
                Origin = result.Origin,
                OriginBn = result.OriginBn,
                Description = result.Description,
                DescriptionBn = result.DescriptionBn,
                Price = result.Price,
                Images = result.Images,
                Height = result.Height,
                Weight =  result.Weight,
                Teeth = result.Teeth,
                
            };
            _logger.LogInformation($"AnimalInfo: {JsonConvert.SerializeObject(result)}");
            //ViewBag.Images = liveAnimalViewModel.Images;
            return View(liveAnimalViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAnimal([Bind] LiveAnimalViewModel model, ICollection<IFormFile> files)
        {
            ViewBag.Categories = await _adminPanelServices.GetCategoryList();
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            var newImages = await _adminPanelServices.UploadImage(files);
            _logger.LogInformation($"UpdateAnimal New Images: {JsonConvert.SerializeObject(newImages)}");

            if (model != null)
            {
                var item = await _adminPanelServices.GetAnimalDetails(model.Id);
                var existingImages = item?.Images;
                if (existingImages == null)
                {
                    existingImages = new List<string>();
                }
                existingImages.AddRange(newImages);
                
                _logger.LogInformation($"UpdateAnimal Final Images: {JsonConvert.SerializeObject(existingImages)}");

                model.Images = existingImages;
            }

            _logger.LogInformation($"UpdateAnimal: {JsonConvert.SerializeObject(model)}");

            await _adminPanelServices.UpdateAnimal(model);
            
            return RedirectToAction("AnimalDetails", "AdminPanel", new{ itemId = model?.Id});
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

        public async Task<bool> RemoveImage(string imgId, string itemId)
        {
            _logger.LogInformation($"Query params: ImgId: {imgId} --- ItemId: {itemId}");
            if (string.IsNullOrEmpty(imgId) || string.IsNullOrEmpty(itemId))
            {
                return false;
            }
            var item = await _adminPanelServices.GetAnimalDetails(itemId);
            _logger.LogInformation($"Item Initial: {JsonConvert.SerializeObject(item)}");
            if (item == null)
            {
                return false;
            }

            var images = item.Images;
            var exist = images.FirstOrDefault(e => e == imgId);
            if (exist != null)
                images.Remove(exist);
            item.Images = images;

            _logger.LogInformation($"Item Final: {JsonConvert.SerializeObject(item)}");
            var res = await _adminPanelServices.UpdateAnimalLiveAnimal(item);
            return res;
        }

        public async Task<IActionResult> PlacedOrder(int? page)
        {
            var OrderViews =  await _orderServices.PlacedOrders();
            var list = OrderViews.ToPagedList(page ?? 1, 9);
            return View(list);
        }

        public async Task<IActionResult> ContactClient(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("PlacedOrder", "AdminPanel");
            }
            bool result = await _orderServices.ContactClient(Id);

            return RedirectToAction("PlacedOrder", "AdminPanel");
        }

        public async Task<IActionResult> OrderDelete(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return RedirectToAction("PlacedOrder", "AdminPanel");
            }

            bool result = await _orderServices.DeleteOrder(itemId);
            if (result)
            {
                return RedirectToAction("PlacedOrder", "AdminPanel");
            }

            return BadRequest("Couldn't delete");
        }
         public async Task<IActionResult> SuccessfulTransaction(int? page)
         { 
             var results = await _transactionService.GetAllTransaction();
             var res = results?.ToList();

             return View(res);
         }

         public async Task<IActionResult> ProblemeticTransaction(int? page)
         {

             var results = await _transactionService.GetAllFailureTransaction();
             var list = results?.ToList();

             return View(list);
         }


    }
}
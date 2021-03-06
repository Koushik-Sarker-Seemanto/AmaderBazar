﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.LiveAnimalModels;
using Models.OrderModels;
using Services.Contracts;
using X.PagedList;
using X.PagedList.Mvc.Core;

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
        public async Task<IActionResult> Index(int? page, int? min, int? max, int? minWeight, int? maxWeight, string category = null, string color = null, string title = null)
        {
            Dictionary<string, int> categoryWise = await _liveAnimalService.GetCategoryCount();
            Dictionary<string, int> colorWise = await _liveAnimalService.GetColorCount();
            ViewBag.CategoryCount = categoryWise;
            ViewBag.ColorCount = colorWise;
            var allAnimals = await _liveAnimalService.GetAllLiveAnimals();
            allAnimals = allAnimals.ToList();

            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            
            if (min != null)
            {
                allAnimals = allAnimals.Where(e => e.Price >= min).ToList();
                queryParam.Add("min", min.ToString());
            }
            if (title != null)
            {
                allAnimals = allAnimals.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                queryParam.Add("title", title);
            }

            if (max != null)
            {
                allAnimals = allAnimals.Where(e => e.Price <= max).ToList();
                queryParam.Add("max", max.ToString());
            }
            if (minWeight != null)
            {
                allAnimals = allAnimals.Where(e => e.Weight >= minWeight).ToList();
                queryParam.Add("minWeight", minWeight.ToString());
            }
            if (maxWeight != null)
            {
                allAnimals = allAnimals.Where(e => e.Weight <= maxWeight).ToList();
                queryParam.Add("maxWeight", maxWeight.ToString());
            }

            if (category != null)
            {
                allAnimals = allAnimals.Where(e => e.Category == category).ToList();
                queryParam.Add("category", category);
            }

            if (color != null)
            {
                allAnimals = allAnimals.Where(e => e.Color == color).ToList();
                queryParam.Add("color", color);
            }

            ViewBag.QueryParam = queryParam;
            var list = allAnimals?.ToPagedList(page ?? 1, 9);
            return View(list);
        }
        public async Task<IActionResult> FeatureProduct(int? page, int? min, int? max, string category = null, string color = null)
        {
            Dictionary<string, int> categoryWise = await _liveAnimalService.GetCategoryCount();
            Dictionary<string, int> colorWise = await _liveAnimalService.GetColorCount();
            ViewBag.CategoryCount = categoryWise;
            ViewBag.ColorCount = colorWise;
            var allAnimals = await _liveAnimalService.GetAllLiveAnimals();
            allAnimals = allAnimals.Where(e=> e.Featured == true).ToList();

            Dictionary<string, string> queryParam = new Dictionary<string, string>();

            if (min != null)
            {
                allAnimals = allAnimals.Where(e => e.Price > min).ToList();
                queryParam.Add("min", min.ToString());
            }

            if (max != null)
            {
                allAnimals = allAnimals.Where(e => e.Price < max).ToList();
                queryParam.Add("max", max.ToString());
            }

            if (category != null)
            {
                allAnimals = allAnimals.Where(e => e.Category == category).ToList();
                queryParam.Add("category", category);
            }

            if (color != null)
            {
                allAnimals = allAnimals.Where(e => e.Color == color).ToList();
                queryParam.Add("color", color);
            }

            ViewBag.QueryParam = queryParam;
            var list = allAnimals?.ToPagedList(page ?? 1, 9);
            return View(list);
        }
        public async Task<IActionResult> NewProduct(int? page, int? min, int? max, string category = null, string color = null)
        {
            Dictionary<string, int> categoryWise = await _liveAnimalService.GetCategoryCount();
            Dictionary<string, int> colorWise = await _liveAnimalService.GetColorCount();
            ViewBag.CategoryCount = categoryWise;
            ViewBag.ColorCount = colorWise;
            var allAnimals = await _liveAnimalService.GetAllLiveAnimals();
            allAnimals = allAnimals.ToList();

            Dictionary<string, string> queryParam = new Dictionary<string, string>();

            if (min != null)
            {
                allAnimals = allAnimals.Where(e => e.Price > min).ToList();
                queryParam.Add("min", min.ToString());
            }

            if (max != null)
            {
                allAnimals = allAnimals.Where(e => e.Price < max).ToList();
                queryParam.Add("max", max.ToString());
            }

            if (category != null)
            {
                allAnimals = allAnimals.Where(e => e.Category == category).ToList();
                queryParam.Add("category", category);
            }

            if (color != null)
            {
                allAnimals = allAnimals.Where(e => e.Color == color).ToList();
                queryParam.Add("color", color);
            }

            ViewBag.QueryParam = queryParam;
            var list = allAnimals?.ToPagedList(page ?? 1, 9);
            return View(list);
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
            // Debug.Print(order.DeliveryDate);
            
            if (ModelState.IsValid == false)
            {
                return View(viewModel);
            }

            var id = Guid.NewGuid().ToString();
            order.Id = id;
            await _orderService.AddOrder(order);
            if (order != null )
            {
                return RedirectToAction("OrderConfirmation", "LiveAnimal", new {orderId = order.Id});
            }
            //Error
            return RedirectToAction("Index", "LiveAnimal");
            
        }

        public async Task<IActionResult> OrderConfirmation(string orderId)
        {
            if (orderId == null)
            {
                //Error
                return RedirectToAction("Index", "LiveAnimal");
            }

            var order = await _orderService.FindOrderById(orderId);
            if (order == null)
            {
                //Error
                return RedirectToAction("Index", "LiveAnimal");
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderConfirmation([Bind] Order order)
        {
            
            if (order != null)
            {
                order = await _orderService.FindOrderById(order.Id);
                if (order == null)
                {
                    return View();
                }
                var animal = await _liveAnimalService.GetLiveAnimalById(order.LiveAnimalId);
                if (animal == null)
                {
                    return View();
                }
                OrderViewModel orderDetailes = new OrderViewModel()
                {
                    Order = order,
                    LiveAnimal = animal,
                };
                return _orderService.CreateReciept(orderDetailes);

            }
            return RedirectToAction("Index");
        }

        

        private async Task<List<LiveAnimalViewModelFrontend>> GetRelated(string category)
        {
            var related = await _liveAnimalService.GetLiveAnimalByCategory(category);
            return related.Take(9).ToList();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class AdminPanelServices: IAdminPanelServices
    {
        private string pathBaseLinux = "wwwroot/images/";
        private string pathBaseWindows = "wwwroot\\images\\";
        private readonly IMongoRepository _repository;
        private readonly ILogger<AdminPanelServices> _logger;
        private IHostingEnvironment _hostingEnvironment;

        public AdminPanelServices(IHostingEnvironment hostingEnvironment,IMongoRepository repository, ILogger<AdminPanelServices> logger)
        {
            _repository = repository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        public async Task<List<Category>> GetCategoryList()
        {
            try
            {
                var categories =  await _repository.GetItemsAsync<Category>();
                return categories?.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetCategoryList Failed: {e.Message}");
                return null;
            }
        }

        public async Task<bool> AddAnimal(LiveAnimalViewModel model)
        {
            try
            {
                var result = await BuildAnimal(model);
                await _repository.SaveAsync<LiveAnimal>(result);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddAnimal Failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAnimal(LiveAnimalViewModel model)
        {
            try
            {
                var id = model.Id;
                var result = await BuildAnimal(model);
                await _repository.UpdateAsync<LiveAnimal>(e => e.Id == id, result);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddAnimal Failed: {e.Message}");
                return false;
            }
        }

        private async Task<LiveAnimal> BuildAnimal(LiveAnimalViewModel model)
        {
            var category = await _repository.GetItemAsync<Category>(e => e.Id == model.Category);
            LiveAnimal liveAnimal = new LiveAnimal
            {
                Id = model.Id,
                Title = model.Title,
                TitleBn =  model.TitleBn,
                Category = category,
                Color = model.Color,
                Location = model.Location,
                LocationBn = model.LocationBn,
                Origin = model.Origin,
                OriginBn = model.OriginBn,
                Price = model.Price,
                Description = model.Description,
                DescriptionBn = model.DescriptionBn,
                Images = model.Images,
                CoverImage = model.CoverImage,
                Featured = model.Featured,
                Weight = model.Weight,
                Height = model.Height,
                Teeth = model.Teeth,
            };
            Dictionary<string,string> color = new Dictionary<string, string>
            {
                {"Black", "কালো"},
                {"White","সাদা" },
                {"Red","লাল" },
                {"Mixed","মিশ্র" },
                {"Ash","খাকি" }
            };
            liveAnimal.ColorBn = color[model.Color];
            return liveAnimal;
        }

        public async Task<bool> AddCategory(Category model)
        {
            try
            {
                await _repository.SaveAsync<Category>(model);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddCategory Failed: {e.Message}");
                return false;
            }
        }

        public async Task<AdminIndexViewModel> GetAnimalList()
        {
            try
            {
                var animals =  await _repository.GetItemsAsync<LiveAnimal>();
                var list = animals?.ToList();
                list?.Reverse();            // New Element upward.
                var response = new AdminIndexViewModel
                {
                    LiveAnimalList = list,
                };
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetCategoryList Failed: {e.Message}");
                return null;
            }
        }

        public async Task<LiveAnimal> GetAnimalDetails(string id)
        {
            try
            {
                var animal =  await _repository.GetItemAsync<LiveAnimal>(e => e.Id == id);
                return animal;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAnimalDetails Failed: {e.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAnimal(string id)
        {
            try
            {
                await _repository.DeleteAsync<LiveAnimal>(e => e.Id == id);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAnimalDetails Failed: {e.Message}");
                return false;
            }
        }

        public async Task<bool> SellAnimal(string itemId)
        {
            _logger.LogInformation("ENTERED ====================== >>>>>");
            try
            {
                var result = await _repository.GetItemAsync<LiveAnimal>(e => e.Id == itemId);
                if (result == null)
                {
                    return false;
                }
                if (result.Sold)
                {
                    _logger.LogInformation($"SellAnimal: Already Sold.");
                    return false;
                }
                result.Sold = true;
                await _repository.UpdateAsync<LiveAnimal>(e => e.Id == result.Id, result);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAnimalDetails Failed: {e.Message}");
                return false;
            }
        }

        public async Task<List<string>> UploadImage(ICollection<IFormFile> files)
        {

            var paths = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    Image uploadedImage;
                    using (var stream = file.OpenReadStream())
                    {
                        uploadedImage = Image.FromStream(stream);
                    }
                    string phrase = file.FileName;
                    string[] words = phrase.Split('.');
                    var Bigimg = ScaleImage(uploadedImage, 960,720);
                    
                    var BigImgPath =  Guid.NewGuid().ToString()+"."+words[1];

                    Bigimg.SaveAs($"{pathBaseLinux}{BigImgPath}");
                    paths.Add(BigImgPath);

                }
            }

            return paths;
        }
        public async Task<string> UploadCoverImage(IFormFile file)
        {

            
            if (file.Length > 0)
                {
                    Image uploadedImage;
                    using (var stream = file.OpenReadStream())
                    {
                        uploadedImage = Image.FromStream(stream);
                    }
                    string phrase = file.FileName;
                    string[] words = phrase.Split('.');
                    var Bigimg = ScaleImage(uploadedImage, 342,342);

                    var BigImgPath = Guid.NewGuid().ToString() + "." + words[1];

                    Bigimg.SaveAs($"{pathBaseLinux}{BigImgPath}");
                    return BigImgPath;

                }

            return "";
        }
        public async Task<bool> UpdateAnimalLiveAnimal(LiveAnimal model)
        {
            try
            {
                var id = model.Id;
                await _repository.UpdateAsync<LiveAnimal>(e => e.Id == id, model);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"UpdateAnimalLiveAnimal Failed: {e.Message}");
                return false;
            }
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight,int maxWeight)
        {
            
            var newImage = new Bitmap(maxHeight, maxWeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, maxHeight, maxWeight);
            }
            return newImage;
        }
    }
}
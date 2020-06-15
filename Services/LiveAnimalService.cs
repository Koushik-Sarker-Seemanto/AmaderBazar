using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Models.LiveAnimalModels;
using Newtonsoft.Json;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class LiveAnimalService : ILiveAnimalService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<LiveAnimalService> _logger;
        public LiveAnimalService(IMongoRepository repository,ILogger<LiveAnimalService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Dictionary<string, int>> GetCategoryCount()
        {
            try
            {
                Dictionary<string, int> data = new Dictionary<string, int>();
                var categories =  await _repository.GetItemsAsync<Category>();
                foreach (var item in categories)
                {
                    var temp = await GetLiveAnimalByCategory(item.Name);
                    data.Add(item.Name, temp.Count);
                }

                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetCategoryCount Failed: {e.Message}");
                return null;
            }
        }

        public async Task<Dictionary<string, int>> GetColorCount()
        {
            try
            {
                Dictionary<string, int> data = new Dictionary<string, int>();
                var animals = await _repository.GetItemsAsync<LiveAnimal>();
                var colors = animals.Select(e => e.Color).Distinct().ToList();

                foreach (var item in colors)
                {
                    var temp = await _repository.GetItemsAsync<LiveAnimal>(e => e.Color == item && e.Sold == false);
                    data.Add(item, temp.Count());
                }

                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetColorCount Failed: {e.Message}");
                return null;
            }
        }

        public async Task<List<LiveAnimalViewModelFrontend>> GetAllLiveAnimals()
        {
            try
            {
                var animals = await _repository.GetItemsAsync<LiveAnimal>();
                var animalsrRes = BuildList(animals?.ToList());
                animalsrRes?.Reverse();
                return animalsrRes;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllAnimalList Failed: {e.Message}");
                return null;
            }
            
        }

        public async Task<LiveAnimalViewModelFrontend> GetLiveAnimalById(string Id)
        {
            try
            {
                var animal = await _repository.GetItemAsync<LiveAnimal>(d => d.Id == Id);
                var animalFront = BuildLiveAnimalViewModelFrontend(animal);
                return animalFront;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllAnimalById Failed: {e.Message}");
                return null;
            }
        }
        /// <summary>
        /// For Related Product Only
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<List<LiveAnimalViewModelFrontend>> GetLiveAnimalByCategory(string category)
        {
            try
            {
                var animals = await _repository.GetItemsAsync<LiveAnimal>(d => d.Category.Name == category );
                var list = animals?.ToList();
                list?.Reverse();
                var animalList = BuildList(list);
                return animalList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllAnimalByCatagory Failed: {e.Message}");
                return null;
            }
            
        }

        public async Task<List<LiveAnimalViewModelFrontend>> GetFeaturedLiveAnimal()
        {
            try
            {
                var animals = await _repository.GetItemsAsync<LiveAnimal>(d => d.Featured == true );
                var list = animals?.ToList();
                var animalList = BuildList(list);
                return animalList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllFeaturedAnimal Failed: {e.Message}");
                return null;
            }
        }

        public async Task<List<LiveAnimalViewModelFrontend>> GetLatestAnimal()
        {
            try
            {
                var animals = await _repository.GetItemsAsync<LiveAnimal>(d => d.Featured == false );
                var list = animals?.ToList();
                list?.Reverse();
                if (list.Count > 8) list.RemoveRange(8,list.Count - 8 );
                
                var animalList = BuildList(list);
                return animalList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllFeaturedAnimal Failed: {e.Message}");
                return null;
            }
        }

        private List<LiveAnimalViewModelFrontend> BuildList(List<LiveAnimal> animals)
        {
            List<LiveAnimalViewModelFrontend> list = new List<LiveAnimalViewModelFrontend>();
            foreach (var animal in animals)
            {
                LiveAnimalViewModelFrontend liveAnimal = BuildLiveAnimalViewModelFrontend(animal);
                if(liveAnimal!= null)
                    list.Add(liveAnimal);
            }

            return list;
        }
        private LiveAnimalViewModelFrontend BuildLiveAnimalViewModelFrontend(LiveAnimal animal)
        {

            if (animal == null) return null;
                LiveAnimalViewModelFrontend liveAnimal = new LiveAnimalViewModelFrontend
                {
                    Id = animal.Id,
                    Title = animal.Title,
                    TitleBn = animal.TitleBn,
                    Category = animal.Category.Name,
                    CategoryBn = animal.Category.NameBn,
                    Color = animal.Color,
                    ColorBn = animal.ColorBn,
                    Location = animal.Location,
                    LocationBn = animal.LocationBn,
                    Origin = animal.Origin,
                    OriginBn = animal.OriginBn,
                    Price = animal.Price,
                    Description = animal.Description,
                    DescriptionBn = animal.DescriptionBn,
                    Images = animal.Images,
                    Sold = animal.Sold,
                    Featured = animal.Featured,
                    Height = animal.Height,
                    Weight = animal.Weight,
                    Teeth = animal.Teeth,
                };
            

            return liveAnimal;
        }

    }
}

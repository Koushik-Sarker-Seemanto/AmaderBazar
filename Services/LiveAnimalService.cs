using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Models.LiveAnimalsModels;
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
        public async Task<List<LiveAnimalViewModelFrontend>> GetLiveAnimalByCategory(string category)
        {
            try
            {
                var animals = await _repository.GetItemsAsync<LiveAnimal>(d => d.Category.Name == category);
                var list = animals?.ToList();
                list?.Reverse();
                var animalList = BuildList(list);
                return animalList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetAllAnimalById Failed: {e.Message}");
                return null;
            }
            
        }
        private List<LiveAnimalViewModelFrontend> BuildList(List<LiveAnimal> animals)
        {
            List<LiveAnimalViewModelFrontend> list = new List<LiveAnimalViewModelFrontend>();
            foreach (var animal in animals)
            {
                LiveAnimalViewModelFrontend liveAnimal = BuildLiveAnimalViewModelFrontend(animal);
                list.Add(liveAnimal);
            }

            return list;
        }
        private LiveAnimalViewModelFrontend BuildLiveAnimalViewModelFrontend(LiveAnimal animal)
        {
            
                LiveAnimalViewModelFrontend liveAnimal = new LiveAnimalViewModelFrontend
                {
                    Id = animal.Id,
                    Title = animal.Title,
                    Category = animal.Category.Name,
                    Color = animal.Color,
                    Location = animal.Location,
                    Origin = animal.Origin,
                    Price = animal.Price,
                    Description = animal.Description,
                };
            

            return liveAnimal;
        }

    }
}

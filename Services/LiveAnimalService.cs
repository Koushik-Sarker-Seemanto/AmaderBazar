using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public LiveAnimalService(IMongoRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<LiveAnimalViewModelFrontend>> GetAllLiveAnimals()
        {
           var animals =  await _repository.GetItemsAsync<LiveAnimal>();
           var animalList = BuildList(animals?.ToList());
           return animalList;
        }

        private List<LiveAnimalViewModelFrontend> BuildList(List<LiveAnimal> animals)
        {
            List<LiveAnimalViewModelFrontend> list = new List<LiveAnimalViewModelFrontend>();
            foreach (var animal in animals)
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
                list.Add(liveAnimal);
            }

            return list;
        }

    }
}

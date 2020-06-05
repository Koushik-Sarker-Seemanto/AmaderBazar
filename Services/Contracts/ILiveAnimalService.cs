using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.LiveAnimalModels;

namespace Services.Contracts
{
    public interface ILiveAnimalService
    {
        Task<Dictionary<string, int>> GetCategoryCount();
        Task<Dictionary<string, int>> GetColorCount();
        Task<List<LiveAnimalViewModelFrontend>> GetAllLiveAnimals();
        Task<LiveAnimalViewModelFrontend> GetLiveAnimalById(string Id);

        Task<List<LiveAnimalViewModelFrontend>> GetLiveAnimalByCategory(string category);
        Task<List<LiveAnimalViewModelFrontend>> GetFeaturedLiveAnimal();
        Task<List<LiveAnimalViewModelFrontend>> GetLatestAnimal();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.AdminModels;
using Models.Entities;

namespace Services.Contracts
{
    public interface IAdminPanelServices
    {
        Task <List<Category>> GetCategoryList();
        Task<bool> AddAnimal(LiveAnimalViewModel model);
        Task<bool> UpdateAnimal(LiveAnimalViewModel model);
        Task<bool> AddCategory(Category model);
        Task<AdminIndexViewModel> GetAnimalList();
        Task<LiveAnimal> GetAnimalDetails(string id);
        Task<bool> DeleteAnimal(string id);
        Task<bool> SellAnimal(string itemId);

        Task<List<string>> UploadImage(ICollection<IFormFile> files);
        Task<bool> UpdateAnimalLiveAnimal(LiveAnimal item);
    }
}
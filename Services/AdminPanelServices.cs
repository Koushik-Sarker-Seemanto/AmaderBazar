using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Models.Entities;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class AdminPanelServices: IAdminPanelServices
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<AdminPanelServices> _logger;
        private IAdminPanelServices _adminPanelServicesImplementation;

        public AdminPanelServices(IMongoRepository repository, ILogger<AdminPanelServices> logger)
        {
            _repository = repository;
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

        private async Task<LiveAnimal> BuildAnimal(LiveAnimalViewModel model)
        {
            var category = await _repository.GetItemAsync<Category>(e => e.Id == model.Category);
            LiveAnimal liveAnimal = new LiveAnimal
            {
                Id = model.Id,
                Title = model.Title,
                Category = category,
                Color = model.Color,
                Location = model.Location,
                Origin = model.Origin,
                Price = model.Price,
                Description = model.Description,
            };
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
                var response = new AdminIndexViewModel
                {
                    LiveAnimalList = animals?.ToList(),
                };
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetCategoryList Failed: {e.Message}");
                return null;
            }
        }
    }
}
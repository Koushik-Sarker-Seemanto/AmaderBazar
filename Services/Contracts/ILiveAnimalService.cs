using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.LiveAnimalsModels;

namespace Services.Contracts
{
    public interface ILiveAnimalService
    {
        Task<List<LiveAnimalViewModelFrontend>> GetAllLiveAnimals();
    }
}

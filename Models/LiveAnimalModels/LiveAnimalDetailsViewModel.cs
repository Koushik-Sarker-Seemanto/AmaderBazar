using System.Collections.Generic;
using Models.Entities;

namespace Models.LiveAnimalModels
{
    public class LiveAnimalDetailsViewModel
    {
        public LiveAnimalViewModelFrontend LiveAnimalDetails { get; set; }
        public List<LiveAnimalViewModelFrontend> Related { get; set; }
        public Order Order { get; set; }
    }
}
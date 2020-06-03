using System;
using System.Collections.Generic;
using System.Text;
using Models.AdminModels;
using Models.Entities;
using Models.LiveAnimalsModels;

namespace Models.OrderModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public LiveAnimalViewModelFrontend LiveAnimal { get; set; }
        

    }
}

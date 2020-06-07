using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.LiveAnimalModels
{
    [Serializable]
    public class LiveAnimalViewModelFrontend
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public double Price { get; set; }
        public string Color { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool Sold { get; set; }
        public bool Featured { get; set; }
        public List<string> Images { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Teeth { get; set; }
    }

   
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Models.AdminModels
{
    [Serializable]
    public class LiveAnimalViewModel
    {
        public string Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        [Required]
        public string TitleBn { get; set; }

        [Required]
        public string Category { get; set; }
        
        [Required]
        public double Price { get; set; }
        public string Color { get; set; }
        public string ColorBn { get; set; }
        public string Origin { get; set; }
        public string OriginBn { get; set; }

        public string Description { get; set; }
        public string DescriptionBn { get; set; }

        public string Location { get; set; }
        public string LocationBn { get; set; }
        public List<string> Images { get; set; }
        public string CoverImage { get; set; }
        
        public bool Featured { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }

       
        public double Teeth { get; set; }


    }
}
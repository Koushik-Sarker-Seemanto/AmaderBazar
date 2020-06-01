using System;
using System.ComponentModel.DataAnnotations;

namespace Models.AdminModels
{
    [Serializable]
    public class LiveAnimalViewModel
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
    }
}
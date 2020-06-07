using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    [Serializable]
    public class LiveAnimal
    {
        [BsonId]
        public string Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        [Required]
        public string TitleBn { get; set; }
        
        [Required]
        public Category Category { get; set; }
        
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
        [DefaultValue(false)]
        public bool Sold { get; set; }

        public bool Featured { get; set; }
        public double Weight { get; set; }

        public double Height { get; set; }

        public double Teeth { get; set; }
        public List<string> Images { get; set; }
    }
}
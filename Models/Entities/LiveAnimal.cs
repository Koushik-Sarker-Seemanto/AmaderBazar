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
        public Category Category { get; set; }
        
        [Required]
        public double Price { get; set; }
        public string Color { get; set; }
        public string Origin { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [DefaultValue(false)]
        public bool Sold { get; set; }

        public List<string> Images { get; set; }
    }
}
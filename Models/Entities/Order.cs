using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    [Serializable]
    public class Order
    {
        [BsonId]
        public string Id { get; set; }

        [Required]
        public string LiveAnimalId { get; set; }
        [Required]
        public  string Name { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "PhoneNumber must be valid and all numeric")]
        [MaxLength(11)]
        [MinLength(11)]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [DefaultValue(false)]
        public bool Contacted { get; set; }
    }
}

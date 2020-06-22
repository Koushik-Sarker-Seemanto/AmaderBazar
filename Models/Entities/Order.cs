using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models.Validation_and_Enums;
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
        [Required]
        public string Address { get; set; }

        [Required]

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [MyDate( ErrorMessage = "Delivery Date Must Be Tommorow or Later")]
        public DateTime DeliveryDate { get; set; }
        [DefaultValue(false)]
        public bool Contacted { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PaymentExpire { get; set; }

    }
}

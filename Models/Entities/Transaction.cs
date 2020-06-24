using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models.Validation_and_Enums;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Models.Entities
{
    public class Transaction
    {
        [Required] public string TranxId { get; set; }
        [Required] public string Name { get; set; }
        
        [Required]
        public double Amount { get; set; }
        public DateTime transactionTime { get; set; }

        [Required] public StatusEnum Status;
        public LiveAnimal Product;
        public Order Order;
    }
}

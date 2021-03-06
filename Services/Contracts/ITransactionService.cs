﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.Validation_and_Enums;

namespace Services.Contracts
{
    public interface ITransactionService
    {
        public Task<bool> AddTransection(string TranxId,string Name,double Amount,StatusEnum Status,LiveAnimal liveAnimal,Order order);
        public Task<List<Transaction>> GetAllTransaction();
        public Task<List<Transaction>> GetAllFailureTransaction();
        public Task<Transaction> GetTransactionByOrderId(string OrderId);
    }
}

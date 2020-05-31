using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Repositories
{
    public interface IMongoRepository
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
        T GetItem<T>(Expression<Func<T, bool>> dataFilters);
        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> dataFilters);
        IQueryable<T> GetItems<T>();
        Task<IQueryable<T>> GetItemsAsync<T>();
        IQueryable<T> GetItems<T>(Expression<Func<T, bool>> dataFilters);
        Task<IQueryable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> dataFilters);
        void Save<T>(T data);
        Task SaveAsync<T>(T data);
        void Save<T>(List<T> data);
        Task SaveAsync<T>(List<T> data);
        void Update<T>(Expression<Func<T, bool>> dataFilters, T data);
        Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, T data);
        void Delete<T>(Expression<Func<T, bool>> dataFilters);
        Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters);

    }
}
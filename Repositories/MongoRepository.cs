using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Repositories
{
    public class MongoRepository: IMongoRepository
    {
        private readonly IMongoDatabase _mongoDatabase;
        public MongoRepository(IDatabaseSettings settings)
        {
            IMongoClient mongoClient = new MongoClient(settings.ConnectionString);
            _mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
        }
        
        /// <summary>
        /// It return MongoDB Collection Object.
        /// </summary>
        /// <typeparam name="T">Db model.</typeparam>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Mongo Collection.</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return this._mongoDatabase.GetCollection<T>(collectionName);
        }

        /// <summary>Gets the item.</summary>
        /// <typeparam name="T">User defined model.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <returns>Passed model as a template.</returns>
        public T GetItem<T>(Expression<Func<T, bool>> dataFilters)
        {
            return this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable().FirstOrDefault(dataFilters);
        }
        
        /// <summary>Gets the item asynchronous.</summary>
        /// <typeparam name="T">Expression.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <returns>Task.</returns>
        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> dataFilters)
        {
            return await this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s")
                .Find(dataFilters).SingleOrDefaultAsync().ConfigureAwait(false);
        }
        
        /// <summary>Gets the items.</summary>
        /// <typeparam name="T">User defined model.</typeparam>
        /// <returns>Passed model to this method.</returns>
        public IQueryable<T> GetItems<T>()
        {
            return this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable();
        }
        
        /// <summary>Gets the items asynchronous.</summary>
        /// <typeparam name="T">object.</typeparam>
        /// <returns>Queryable task.</returns>
        public async Task<IQueryable<T>> GetItemsAsync<T>()
        {
            return await Task.Run(() => 
                this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable()).ConfigureAwait(false);
        }
        
        /// <summary>Gets the items.</summary>
        /// <typeparam name="T">Model want to pass.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <returns>User defined model.</returns>
        public IQueryable<T> GetItems<T>(Expression<Func<T, bool>> dataFilters)
        {
            return this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable().Where(dataFilters);
        }
        /// <summary>Gets the items asynchronous.</summary>
        /// <typeparam name="T">Expression.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <returns>Queryable task.</returns>
        public async Task<IQueryable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> dataFilters)
        {
            return await Task.Run(() =>
            {
                return this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").AsQueryable().Where(dataFilters);
            }).ConfigureAwait(false);
        }

        /// <summary>Saves the specified data.</summary>
        /// <typeparam name="T">User defined model.</typeparam>
        /// <param name="data">The data.</param>
        public void Save<T>(T data)
        {
            this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").InsertOne(data);
        }
        
        /// <summary>Saves the asynchronous.</summary>
        /// <typeparam name="T">Model.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns><Type>A <see cref="Task"/> representing the asynchronous operation.</Type></returns>
        public async Task SaveAsync<T>(T data)
        {
            await this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").InsertOneAsync(data).ConfigureAwait(false);
        }

        /// <summary>Saves the specified data.</summary>
        /// <typeparam name="T">User defined model.</typeparam>
        /// <param name="data">The data.</param>
        public void Save<T>(List<T> data)
        {
            this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").InsertMany(data);
        }
        
        /// <summary>Saves the asynchronous.</summary>
        /// <typeparam name="T">List of object.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns><Task>A <see cref="Task"/> representing the asynchronous operation.</Task></returns>
        public async Task SaveAsync<T>(List<T> data)
        {
            await this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").InsertManyAsync(data).ConfigureAwait(false);
        }
        
        /// <summary>Updates the specified data filters.</summary>
        /// <typeparam name="T">Used defined model.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <param name="data">The data.</param>
        public void Update<T>(Expression<Func<T, bool>> dataFilters, T data)
        {
            this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").ReplaceOne(dataFilters, data);
        }
        
        /// <summary>Updates the asynchronous.</summary>
        /// <typeparam name="T">Expression.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        /// <param name="data">The data.</param>
        public async Task UpdateAsync<T>(Expression<Func<T, bool>> dataFilters, T data)
        {
            await this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").ReplaceOneAsync(dataFilters, data).ConfigureAwait(false);
        }

        /// <summary>Deletes the specified data filters.</summary>
        /// <typeparam name="T">User defined model.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        public void Delete<T>(Expression<Func<T, bool>> dataFilters)
        {
            this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").DeleteMany(dataFilters);
        }

        /// <summary>Deletes the asynchronous.</summary>
        /// <typeparam name="T">Expression.</typeparam>
        /// <param name="dataFilters">The data filters.</param>
        public async Task DeleteAsync<T>(Expression<Func<T, bool>> dataFilters)
        {
            await this._mongoDatabase.GetCollection<T>($"{typeof(T).Name}s").DeleteManyAsync(dataFilters).ConfigureAwait(false);
        }
    }
}
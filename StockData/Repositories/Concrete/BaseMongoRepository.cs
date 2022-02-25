using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StockData.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StockData.Repositories.Concrete
{
    public class BaseMongoRepository<T> : IBaseMongoRepository<T> where T:class
        {
        private readonly IConfiguration _config;
        private MongoClient _client;
        private IMongoDatabase _database;

        private string _conStr;
        private string _databaseName;

        private Dictionary<string, object> _collections = new Dictionary<string, object>();

        public BaseMongoRepository(IConfiguration config)
        {
            _config = config;

            _conStr = _config["MongoDbSettings:ConnectionString"];
            _databaseName = _config["MongoDbSettings:DatabaseName"];

            _client = new MongoClient(_conStr);
            _database = _client.GetDatabase(_databaseName);
        }

        public IMongoCollection<T> GetProperCollection()
        {
            IMongoCollection<T> collection;
            Type type = typeof(T);
            string collectionName = type.Name;
            if (_collections.ContainsKey(collectionName))
            {
                collection = _collections[collectionName] as IMongoCollection<T>;
            }
            else
            {
                collection = _database.GetCollection<T>(collectionName);
                _collections.Add(collectionName, collection);

                collection = _collections[collectionName] as IMongoCollection<T>;
            }

            return collection;
        }

        public IMongoCollection<T> GetProperCollection(string collectionName)
        {
            IMongoCollection<T> collection;
            if (_collections.ContainsKey(collectionName))
            {
                collection = _collections[collectionName] as IMongoCollection<T>;
            }
            else
            {
                collection = _database.GetCollection<T>(collectionName);
                _collections.Add(collectionName, collection);

                collection = _collections[collectionName] as IMongoCollection<T>;
            }

            return collection;
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter)
        {
            IMongoCollection<T> collection = GetProperCollection();
            var response = await collection.FindAsync<T>(filter);
            return response.FirstOrDefault();
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            IMongoCollection<T> collection = GetProperCollection();

            var result = await collection.FindAsync<T>(filter);
            int count = result.ToList().Count;
            return count > 0;
        }      

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IMongoCollection<T> collection = GetProperCollection();

            return await collection.FindAsync(x => true).Result.ToListAsync();
        }      

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            IMongoCollection<T> collection = GetProperCollection();
            var list = await collection.FindAsync<T>(filter);
            return list.ToList();
        }

        public async Task InsertOneAsync(T model)
        {
            IMongoCollection<T> collection = GetProperCollection();

            await collection.InsertOneAsync(model);
        }
        public async Task UpdateOneAsync(Expression<Func<T, bool>> filter, T model)
        {
            IMongoCollection<T> collection = GetProperCollection();

            await collection.FindOneAndReplaceAsync<T>(filter, model);
        }




    }
}
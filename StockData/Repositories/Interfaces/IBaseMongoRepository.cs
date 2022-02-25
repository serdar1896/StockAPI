using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StockData.Repositories.Interfaces
{
    public interface IBaseMongoRepository<T> where T : class    
    {
        IMongoCollection<T> GetProperCollection();

        IMongoCollection<T> GetProperCollection (string collectionName);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        
        Task<IEnumerable<T>> GetAllAsync();

        Task<List<T>> GetAllAsync (Expression<Func<T, bool>> filter);


        Task<T> GetOneAsync (Expression<Func<T, bool>> filter);
        
        Task InsertOneAsync (T model);
       
        Task UpdateOneAsync (Expression<Func<T, bool>> filter, T model);        
  
    }
}

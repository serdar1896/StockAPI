using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockData.Repositories.Interfaces
{
    public interface IRedisRepository
    {
        Task<T> GetAsync<T>(string key);
        Task<IEnumerable<T>> GetByPatternAsync<T>(string pattern);
        Task AddAsync(string key, object data);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task Clear();
        Task<bool> AnyAsync(string key);
    }
}

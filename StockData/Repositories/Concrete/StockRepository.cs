using Microsoft.Extensions.Configuration;
using StockData.Entities;
using StockData.Repositories.Interfaces;

namespace StockData.Repositories.Concrete
{
    public class StockRepository:BaseMongoRepository<Stock> , IStockRepository
    {
        public StockRepository(IConfiguration config) : base(config)
        {
        }
    }
}

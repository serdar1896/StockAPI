using StockData.Entities;

namespace StockData.Repositories.Interfaces
{
    public interface IStockRepository:IBaseMongoRepository<Stock>
    {
    }
}

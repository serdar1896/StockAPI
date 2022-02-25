using StockService.Models.RequestModels;
using StockService.Models.ResponseModels;
using System.Threading.Tasks;

namespace StockService.Interfaces
{
    public interface IStockService
    {
        Task InsertOrUpdateStockAsync(StockRequestModel stock);
        Task<bool> AnyUniqueVariantCodeAsync(StockRequestModel stock);
        Task<StockResponseModel> GetStockByProductCodeAsync(string productCode);
        Task<StockResponseModel> GetStockByVariantCodeAsync(string variantCode);
    }
}

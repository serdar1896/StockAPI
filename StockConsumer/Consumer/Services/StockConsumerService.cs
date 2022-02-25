using Newtonsoft.Json;
using StockService.Interfaces;
using StockService.Models.RequestModels;

namespace StockConsumer.Consumer.Services
{
    public class StockConsumerService
    {
        private readonly IStockService _stockService;
        public StockConsumerService(IStockService stockService)
        {
            _stockService = stockService;
        }
        public void InsertorUpdateStock( string message) 
        {
            var stockRequestModel = JsonConvert.DeserializeObject<StockRequestModel>(message);
             _stockService.InsertOrUpdateStockAsync(stockRequestModel);

        }

    }
}

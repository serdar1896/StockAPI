using System.Collections.Generic;

namespace StockService.Models.ResponseModels
{
    public class StockResponseModel
    {
        public  int TotalStock { get; set; }
        public List<StockModel> StockItems { get; set; } 
    }   
}

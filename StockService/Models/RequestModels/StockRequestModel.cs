namespace StockService.Models.RequestModels
{
    public class StockRequestModel
    {
        public string ProductCode { get; set; }

        public string VariantCode { get; set; }

        public int Quantity { get; set; }
    }
}

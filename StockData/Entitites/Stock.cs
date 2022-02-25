using StockData.Entitites;

namespace StockData.Entities
{
    public class Stock:BaseEntity<string>
    {
        public string ProductCode { get; set; }

        public string VariantCode { get; set; }

        public int Quantity { get; set; }

    }
}

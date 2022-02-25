using System.ComponentModel;

namespace StockService.Models
{
    public class StockModel:BaseModel<string>
    {
        /// <summary>
        /// Ürün Kodu
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Varyant Kodu
        /// </summary>
        public string VariantCode { get; set; }

        /// <summary>
        /// Stok Miktarı
        /// </summary>
        [Description("Stok Miktarı")]
        public int Quantity { get; set; }
    }
}

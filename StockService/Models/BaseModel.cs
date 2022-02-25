using System;
using System.ComponentModel.DataAnnotations;

namespace StockService.Models
{
    public class BaseModel<TKey>
    {
        public TKey Id { get; set; }
    }
}

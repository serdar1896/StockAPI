using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockData.Enum;
using StockService.Helpers;
using StockService.Interfaces;
using StockService.Models.RequestModels;
using StockService.Models.ResponseModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly RabbitHelper _rabbitHelper;
        public StocksController(IStockService stockService, RabbitHelper rabbitHelper)
        {
            _stockService = stockService;
            _rabbitHelper = rabbitHelper;
        }

        /// <summary>
        /// Get: /GetStockByVariantCode
        /// </summary>
        /// <param name="variantCode"></param>
        /// <returns></returns>
        [Description("Get Stock By Variant Code")]
        [Produces("application/json")]
        [HttpGet("{variantCode}/variant")]
        public async Task<ActionResult<StockResponseModel>> GetStockByVariantCode(string variantCode)
        {
            return Ok(await _stockService.GetStockByVariantCodeAsync(variantCode));
        }

        /// <summary>
        /// Get: /GetStockByProductCode
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        [Description("Get Stock By Product Code")]
        [Produces("application/json")]
        [HttpGet("{storeCode}/product")]
        public async Task<ActionResult<StockResponseModel>> GetStockByProductCode(string storeCode)
        {
            return Ok(await _stockService.GetStockByProductCodeAsync(storeCode));
        }

        /// <summary>
        /// PATCH: /InsertOrUpdateStock
        /// </summary>
        [Description("Insert Or Update Stock")]
        [HttpPost]
        public async Task<IActionResult> InsertOrUpdateStock(StockRequestModel requestModel)
        {
            var anyVariantCode = await _stockService.AnyUniqueVariantCodeAsync(requestModel);
            if (anyVariantCode)
            {
                return BadRequest(ErrorCodes.WrongVariant.Text);
            }
            _rabbitHelper.SetMessage("StockQueue", JsonConvert.SerializeObject(requestModel));
            return Ok();
        }
    }
}

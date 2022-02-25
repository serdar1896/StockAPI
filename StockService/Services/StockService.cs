using AutoMapper;
using StockData.Entities;
using StockService.Models.RequestModels;
using StockData.Repositories.Interfaces;
using System.Threading.Tasks;
using StockService.Interfaces;
using StockService.Models.ResponseModels;
using System.Linq;
using System.Collections.Generic;
using System;
using StockService.Models;
using StockData.Enum;

namespace StockService.Services
{
    public class StockService : IStockService
      {
        private readonly IStockRepository _repository;
        private readonly IMapper _mapper;
        private readonly IRedisRepository _redisService;

        public StockService(IStockRepository repository, IMapper mapper, IRedisRepository redisService)
        {
            _repository = repository;
            _mapper = mapper;
            _redisService = redisService;
          
        }

        public async Task<bool> AnyUniqueVariantCodeAsync(StockRequestModel stockRequestModel)
        {
            var productsStock = await _repository.AnyAsync(x => x.ProductCode == stockRequestModel.ProductCode && x.VariantCode == stockRequestModel.VariantCode);
            if (!productsStock)
            {
                var uniqeVariantCode = await _repository.AnyAsync(x => x.VariantCode == stockRequestModel.VariantCode);
                if (uniqeVariantCode)
                {
                    return true;

                }
            }
            return false;
        }

        public async Task<StockResponseModel> GetStockByProductCodeAsync(string productCode)
        {

            var response = await _redisService.GetAsync<StockResponseModel>("stock_product_" + productCode);
            if (response==null || response.StockItems.Count==0)
            {
                response = new StockResponseModel();

                var stocks = await _repository.GetAllAsync(x => x.ProductCode == productCode);
                response.TotalStock = stocks.ToList().Sum(x => x.Quantity);
                response.StockItems = new List<StockModel>(_mapper.Map<List<StockModel>>(stocks));
                await _redisService.AddAsync("stock_product_" + productCode, response);

                return response;
            }
            return response;
        }

        public async Task<StockResponseModel> GetStockByVariantCodeAsync(string variantCode)
        {
            var response = await _redisService.GetAsync<StockResponseModel>("stock_variant_" + variantCode);
            if (response ==null || response.StockItems.Count==0)
            {
                response = new StockResponseModel();
                var stocks = await _repository.GetAllAsync(x => x.VariantCode == variantCode);
                response.TotalStock = stocks.ToList().Sum(x => x.Quantity);
                response.StockItems = new List<StockModel>();
                response.StockItems = _mapper.Map<List<StockModel>>(stocks);
                await _redisService.AddAsync("stock_variant_" + variantCode, response);

                return  response;
            }
            return response;
        }

        public async Task InsertOrUpdateStockAsync(StockRequestModel stockRequestModel)
        {
            if (stockRequestModel == null)
                throw new ArgumentNullException(nameof(stockRequestModel));

            var dto = new Stock();

            var productsStock = await _repository.GetOneAsync(x => x.ProductCode == stockRequestModel.ProductCode && x.VariantCode == stockRequestModel.VariantCode);
            if (productsStock == null)
            {
                var uniqeVariantCode = await  _repository.AnyAsync(x=>x.VariantCode==stockRequestModel.VariantCode);
                if (uniqeVariantCode)
                {
                   throw new ArgumentException( ErrorCodes.WrongVariant.Text);
                }
                dto= _mapper.Map<Stock>(stockRequestModel);
                dto.InsertedDate = DateTime.Now;
                await _repository.InsertOneAsync(dto);
                return;
            }
            dto = productsStock;
            dto.Quantity += stockRequestModel.Quantity ;
 
            await _repository.UpdateOneAsync(x => x.Id == productsStock.Id,dto);
            await _redisService.RemoveAsync("stock_variant_" + stockRequestModel.VariantCode);
            await _redisService.RemoveAsync("stock_product_"+ stockRequestModel.ProductCode);
        }

    }
}

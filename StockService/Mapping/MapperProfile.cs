using AutoMapper;
using StockData.Entities;
using StockService.Models;
using StockService.Models.RequestModels;

namespace StockService.Mapping
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Stock,StockRequestModel>();

            CreateMap<StockRequestModel, Stock>();

            CreateMap<Stock, StockModel>();

            CreateMap<StockModel, Stock>();



        }
    }
}

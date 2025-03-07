using AutoMapper;
using Calia.Services.StockAPI.Models;
using Calia.Services.StockAPI.Models.Dto;
using System.Runtime;

namespace Calia.Services.StockAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfing = new MapperConfiguration(config =>
            {
                
				config.CreateMap<StockMaterial, StockMaterialDto>().ReverseMap();
            });
            return mappingConfing;
        } 
    }
}

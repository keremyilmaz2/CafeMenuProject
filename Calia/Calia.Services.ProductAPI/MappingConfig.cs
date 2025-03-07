using AutoMapper;
using Calia.Services.ProductAPI.Models;
using Calia.Services.ProductAPI.Models.Dto;
using System.Runtime;

namespace Calia.Services.ProductAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfing = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
				config.CreateMap<ProductMaterial, ProductMaterialDto>().ReverseMap();
				config.CreateMap<ProductExtraDto, ProductExtra>().ReverseMap();
			});
            return mappingConfing;
        } 
    }
}

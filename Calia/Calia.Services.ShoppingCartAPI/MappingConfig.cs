using AutoMapper;
using Calia.Services.ShoppingCartAPI.Models;
using Calia.Services.ShoppingCartAPI.Models.Dto;
using System.Runtime;

namespace Calia.Services.ShoppingCartAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfing = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                config.CreateMap<ShoppingCartExtra, ShoppingCartExtraDto>().ReverseMap();
            });
            return mappingConfing;
        } 
    }
}

using AutoMapper;
using Calia.Services.OrderAPI.Models;
using Calia.Services.OrderAPI.Models.Dto;
using System.Runtime;

namespace Calia.Services.OrderAPI
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfing = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto,CartHeaderDto>()
                .ForMember(dest=>dest.CartTotal,u=>u.MapFrom(src=>src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailsDto,OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();
                config.CreateMap<TableNo, TableNoDto>().ReverseMap();
                config.CreateMap<TableDetails, TableDetailsDto>().ReverseMap();
                config.CreateMap<OrderHeader,OrderHeaderDto>().ReverseMap();
                config.CreateMap<CancelDetails, CancelDetailsDto>().ReverseMap();
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
                config.CreateMap<OrderExtra, OrderExtraDto>().ReverseMap();
                config.CreateMap<ShoppingCartExtraDto, OrderExtraDto>().ReverseMap();
                config.CreateMap<KisiselGider, KisiselGiderDto>().ReverseMap();
                config.CreateMap<AdminNames, AdminNamesDto>().ReverseMap();
                config.CreateMap<Veriler, VerilerDto>().ReverseMap();
            });
            return mappingConfing;
        } 
    }
}

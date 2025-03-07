using AutoMapper;
using Calia.Services.CategoryAPI.Models;
using Calia.Services.CategoryAPI.Models.Dto;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CategoryDto, Category>().ReverseMap();
            config.CreateMap<CategoryMaterial, CategoryMaterialDto>().ReverseMap();
            config.CreateMap<CategoryExtra, CategoryExtraDto>().ReverseMap();
        });

        return mappingConfig;
    }
}

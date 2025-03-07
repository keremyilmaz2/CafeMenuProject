using Calia.Services.CategoryAPI.Models.Dto;

namespace Calia.Services.ProductAPI.Service.IService
{
    public interface ICategoryService
    {
		Task<CategoryDto> GetCategory(int id);
	}
}

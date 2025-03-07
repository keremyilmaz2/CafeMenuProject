using Calia.Services.ProductAPI.Models.Dto;

namespace Calia.Services.ProductAPI.Service.IService
{
    public interface ICategoryService
    {
		Task<CategoryDto> GetCategory(int id);
        Task<List<CategoryDto>> GetAllCategories();
	}
}

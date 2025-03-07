using Calia.Services.CategoryAPI.Models.Dto;

namespace Calia.Services.ProductAPI.Service.IService
{
    public interface StockService
    {
		Task<CategoryDto> GetCategory(int id);
	}
}

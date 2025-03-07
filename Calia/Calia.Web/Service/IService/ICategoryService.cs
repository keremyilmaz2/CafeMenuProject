using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
	public interface ICategoryService
	{
		Task<ResponseDto?> GetAllCategoriesAsync();
		Task<ResponseDto?> GetAllCategoriesMaterialAsync();
        Task<ResponseDto?> GetAllCategoriesExtraAsync();
        Task<ResponseDto?> GetCategoryByIdAsync(int id);
		Task<ResponseDto?> CategoryMaterialGetForcategoryMaterialVm(string name);
        Task<ResponseDto?> CategoryExtraGetForcategoryExtraVm(string name);
        Task<ResponseDto?> CreateCategoryAsync(CategoryDto categoryDto);
		Task<ResponseDto?> CreateCategoryMaterialAsync(CategoryMaterialDto categoryMaterialsDto);
		Task<ResponseDto?> CreateCategoryExtraAsync(CategoryExtraDto categoryExtraDto);
		Task<ResponseDto?> UpdateCategoryAsync(CategoryDto categoryDto);
        Task<ResponseDto?> UpdateCategoryMatiralList(CategoryMaterialVM categoryMaterialVM);
        Task<ResponseDto?> UpdateCategoryExtraList(CategoryExtraVM CategoryExtraVM);
        Task<ResponseDto?> DeleteCategoryAsync(int id);
        Task<ResponseDto?> DeleteCategoryMaterialAsync(int id);
    }
}

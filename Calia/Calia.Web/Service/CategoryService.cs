using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;

namespace Calia.Web.Service
{
	public class CategoryService : ICategoryService
	{
		private readonly IBaseService _baseService;
		public CategoryService(IBaseService baseService)
		{
			_baseService = baseService;
		}

        public async Task<ResponseDto?> CategoryExtraGetForcategoryExtraVm(string name)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CategoryAPIBase + "/api/category/CategoryExtraGetForcategoryExtraVm/" + name,
            });
        }

        public async Task<ResponseDto?> CategoryMaterialGetForcategoryMaterialVm(string name)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CategoryAPIBase + "/api/category/CategoryMaterialGetForcategoryMaterialVm/" + name,
            });
        }

        public async Task<ResponseDto?> CreateCategoryAsync(CategoryDto categoryDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = categoryDto,
				Url = SD.CategoryAPIBase + "/api/category"
			});
		}

		public async Task<ResponseDto?> CreateCategoryExtraAsync(CategoryExtraDto categoryExtraDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = categoryExtraDto,
				Url = SD.CategoryAPIBase + "/api/category/PostExtraMaterial"
			});
		}

		public async Task<ResponseDto?> CreateCategoryMaterialAsync(CategoryMaterialDto categoryMaterialsDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = categoryMaterialsDto,
				Url = SD.CategoryAPIBase + "/api/category/PostCategoryMaterial"
			});
		}

		public async Task<ResponseDto?> DeleteCategoryAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.DELETE,
				Url = SD.CategoryAPIBase + "/api/category/" + id
			});
		}

        public async Task<ResponseDto?> DeleteCategoryMaterialAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CategoryAPIBase + "/api/category/DeleteMaterial/" + id
            });
        }

        public async Task<ResponseDto?> GetAllCategoriesAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CategoryAPIBase + "/api/category"
			});
		}

        public async Task<ResponseDto?> GetAllCategoriesExtraAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CategoryAPIBase + "/api/category/CategoryExtraGetAll"
            });
        }

        public async Task<ResponseDto?> GetAllCategoriesMaterialAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CategoryAPIBase + "/api/category/CategoryMaterialGetAll"
            });
		}

		public async Task<ResponseDto?> GetCategoryByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.CategoryAPIBase + "/api/category/" + id
			});
		}

		public async Task<ResponseDto?> UpdateCategoryAsync(CategoryDto categoryDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.PUT,
				Data = categoryDto,
				Url = SD.CategoryAPIBase + "/api/category"
			});
		}

        public async Task<ResponseDto?> UpdateCategoryExtraList(CategoryExtraVM CategoryExtraVM)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CategoryExtraVM,
                Url = SD.CategoryAPIBase + "/api/category/EditCategoryExtraList"
            });
        }

        public async Task<ResponseDto?> UpdateCategoryMatiralList(CategoryMaterialVM categoryMaterialVM)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = categoryMaterialVM,
                Url = SD.CategoryAPIBase + "/api/category/EditCategoryMaterialList"
            });
        }
    }
}

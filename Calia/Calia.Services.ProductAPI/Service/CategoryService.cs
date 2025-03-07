using Calia.Services.ProductAPI.Models.Dto;
using Calia.Services.ProductAPI.Service.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Calia.Services.ProductAPI.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CategoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;  
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var client = _httpClientFactory.CreateClient("Category");
            var response = await client.GetAsync($"/api/category");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<List<CategoryDto>>(Convert.ToString(resp.Result));
            }
            return new List<CategoryDto>();
        }

        public async Task<CategoryDto> GetCategory(int id)
        {
            var client = _httpClientFactory.CreateClient("Category");
            var response = await client.GetAsync($"/api/category/{id}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CategoryDto>(Convert.ToString(resp.Result));
            }
            return new CategoryDto();
        }
    }
}

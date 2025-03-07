using Calia.Services.OrderAPI.Models.Dto;
using Calia.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Calia.Services.OrderAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;  
        }

        public async Task<ResponseDto?> DropAvailableProduct(OrderDetailsDto orderDetailsDto)
        {
            var client = _httpClientFactory.CreateClient("Product");

            // Request data with a JSON body
            var content = new StringContent(
                JsonConvert.SerializeObject(orderDetailsDto),
                Encoding.UTF8,
                "application/json"
            );

            // Sending POST request to the API
            var response = await client.PutAsync($"/api/product/DropProductCount", content);

            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (resp.IsSuccess)
            {
                return resp;
            }
            return new ResponseDto { IsSuccess = false, Message = "Material creation failed." };
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
        }
    }
}

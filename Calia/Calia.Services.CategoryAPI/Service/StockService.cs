using Calia.Services.CategoryAPI.Models.Dto;
using Calia.Services.CategoryAPI.Service.IService;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Calia.Services.CategoryAPI.Service
{
    public class StockService : IStockService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public StockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;  
        }

		public async Task<ResponseDto?> CreateStockMaterial(string stockMaterialName)
		{
			var client = _httpClientFactory.CreateClient("Stock");

			// Request data with a JSON body
			var content = new StringContent(
				JsonConvert.SerializeObject(stockMaterialName),
				Encoding.UTF8,
				"application/json"
			);

			// Sending POST request to the API
			var response = await client.PostAsync("/api/stock/Creatematerial", content);

			var apiContent = await response.Content.ReadAsStringAsync();
			var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			if (resp.IsSuccess)
			{
				return resp;
			}
			return new ResponseDto { IsSuccess = false, Message = "Material creation failed." };
		}


	}
}

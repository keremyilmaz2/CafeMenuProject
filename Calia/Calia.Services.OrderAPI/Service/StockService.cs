using Calia.Services.OrderAPI.Models.Dto;
using Calia.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Calia.Services.OrderAPI.Service
{
	public class StockService :IStockService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public StockService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<ResponseDto?> DecreaseStockMaterial(string materialName, int materialAmount)
		{
			var client = _httpClientFactory.CreateClient("Stock");

			var content = new StringContent(
				JsonConvert.SerializeObject(materialAmount),
				Encoding.UTF8,
				"application/json"
			);

			var response = await client.PutAsync($"/api/Stock/DecreaseStockMaterial/{materialName}", content);

			var apiContent = await response.Content.ReadAsStringAsync();
			var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			if (resp.IsSuccess)
			{
				return resp;
			}
			return new ResponseDto { IsSuccess = false, Message = "Stock decrease failed." };
		}


        public async Task<IEnumerable<StockMaterialDto>> GetStockMaterialDtosAsync()
        {
            var client = _httpClientFactory.CreateClient("Stock");
            var response = await client.GetAsync($"/api/stock");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<StockMaterialDto>>(Convert.ToString(resp.Result));
            }
            return new List<StockMaterialDto>();
        }

    }
}

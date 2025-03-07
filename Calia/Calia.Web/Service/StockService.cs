using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;

namespace Calia.Web.Service
{
	public class StockService : IStockService

	{
		private readonly IBaseService _baseService;
		public StockService(IBaseService baseService)
		{
			_baseService = baseService;
		}
		public async Task<ResponseDto?> CreateStockAsync(string stockMaterialName)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = stockMaterialName,
				Url = SD.StockAPIBase + "/api/stock/Creatematerial"
			});
		}

		

		public async Task<ResponseDto?> GetAllStocksAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.StockAPIBase + "/api/stock"
			});
		}

		public async Task<ResponseDto?> GetStockByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.StockAPIBase + "/api/stock/" + id
			});
		}

		public async Task<ResponseDto?> UpdateStockAsync(StockMaterialDto stockMaterialDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.PUT,
				Data = stockMaterialDto,
				Url = SD.StockAPIBase + "/api/stock/ChangeMaterialAmount/" + stockMaterialDto.MaterialId
			});
		}
	}
}

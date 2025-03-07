using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
	public interface IStockService
	{
		Task<ResponseDto?> GetAllStocksAsync();
		Task<ResponseDto?> GetStockByIdAsync(int id);
		Task<ResponseDto?> CreateStockAsync(string stockMaterialName);
		Task<ResponseDto?> UpdateStockAsync(StockMaterialDto stockMaterialDto);
		
	}
}

using Calia.Services.OrderAPI.Models.Dto;

namespace Calia.Services.OrderAPI.Service.IService
{
	public interface IStockService
	{
		Task<ResponseDto?> DecreaseStockMaterial(string materialName, int materialAmount);
        Task<IEnumerable<StockMaterialDto>> GetStockMaterialDtosAsync();
    }
}

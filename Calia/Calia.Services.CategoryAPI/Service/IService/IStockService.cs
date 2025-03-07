

using Calia.Services.CategoryAPI.Models.Dto;

namespace Calia.Services.CategoryAPI.Service.IService
{
    public interface IStockService
    {
		Task<ResponseDto?> CreateStockMaterial(string stockMaterialName);
	}
}

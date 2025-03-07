using Calia.Services.ProductAPI.Models.Dto;

namespace Calia.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}

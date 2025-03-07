using Calia.Services.ShoppingCartAPI.Models.Dto;

namespace Calia.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}

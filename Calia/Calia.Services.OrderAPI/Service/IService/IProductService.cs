using Calia.Services.OrderAPI.Models.Dto;

namespace Calia.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ResponseDto?> DropAvailableProduct(OrderDetailsDto orderDetailsDto);
    }
}

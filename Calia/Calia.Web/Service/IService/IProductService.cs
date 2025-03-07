using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
    public interface IProductService
    {
		Task<ResponseDto?> GetAllProductsAsync();
		Task<ResponseDto?> GetProductByIdAsync(int id);
		Task<ResponseDto?> CreateProductAsync(ProductVM productVM);
		Task<ResponseDto?> UpdateProductAsync(ProductVM productVM);
		Task<ResponseDto?> DeleteProductAsync(int id);
		Task<ResponseDto?> AddProductCount(ProductDto productDto);
        Task<ResponseDto?> GetProductByCategoryIdAsync(int categoryId);
        //Yeniler
        Task<ResponseDto?> ProductCreateViewForVm();
        Task<ResponseDto?> ProductEditViewForVm(int id);
        Task<ResponseDto?> ProductMaterialAndExtraViewForVm(int id);

    }
}

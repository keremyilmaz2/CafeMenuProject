using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartVM cartVM);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto?> Plus(int cartDetailsId);
        Task<ResponseDto?> Minus(int cartDetailsId);
        Task<ResponseDto?> RemoveShoppingCart(int cartHeaderId);




    }
}

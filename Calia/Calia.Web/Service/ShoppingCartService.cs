using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;

namespace Calia.Web.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;
        public ShoppingCartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        
        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/GetCart/" + userId
            });
        }

        public async Task<ResponseDto?> Minus(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/MinusCart"
            });
        }

        public async Task<ResponseDto?> Plus(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/PlusCart"
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> RemoveShoppingCart(int cartHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeaderId,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/RemoveShoppingCart"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartVM cartVM)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartVM,
                Url = SD.ShoppingCartAPIBase + "/api/ShoppingCart/CartUpsert"
            });
        }
    }
}

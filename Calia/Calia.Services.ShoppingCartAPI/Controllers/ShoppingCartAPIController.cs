using AutoMapper;
using Calia.Services.ShoppingCartAPI.Data;
using Calia.Services.ShoppingCartAPI.Hubs;
using Calia.Services.ShoppingCartAPI.Models;
using Calia.Services.ShoppingCartAPI.Models.Dto;
using Calia.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.ShoppingCartAPI.Controllers
{
	[Route("api/ShoppingCart")]
	[ApiController]
	public class ShoppingCartAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
        private readonly IHubContext<CartHub> _cartHub;
        private ResponseDto _response;
		private IMapper _mapper;
		private IProductService _productService;
		private readonly IConfiguration _configuration;
        private readonly ILogger<ShoppingCartAPIController> _logger;

        public ShoppingCartAPIController(AppDbContext db, IMapper mapper, IHubContext<CartHub> cartHub, IProductService productService, IConfiguration configuration, ILogger<ShoppingCartAPIController> logger)
		{
			_db = db;
			_mapper = mapper;
            _cartHub = cartHub;
			_productService = productService;
			_configuration = configuration;
            _logger = logger;
            _response = new ResponseDto();
		}






        [HttpGet("GetCart/{sessionId}")]
        public async Task<ResponseDto> GetCart(string sessionId)
        {
            _logger.LogInformation("GetCart çağrıldı. SessionId: {SessionId}", sessionId);

            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == sessionId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    item.ProductExtrasList = _mapper.Map<List<ShoppingCartExtraDto>>(
                        _db.CartDetails.Include(u => u.ProductExtrasList)
                        .FirstOrDefault(u => u.CartDetailsId == item.CartDetailsId)?.ProductExtrasList);

                    cart.CartHeader.CartTotal += (item.Count * item.DetailPrice);
                }

                _response.Result = cart;
                _logger.LogInformation("GetCart işlemi başarıyla tamamlandı. SessionId: {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCart hata verdi. SessionId: {SessionId}", sessionId);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("GetCount/{userId}")]
        public async Task<IActionResult> GetCount(string userId)
        {
            _logger.LogInformation("GetCount çağrıldı. UserId: {UserId}", userId);

            try
            {
                var cartHeader = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
                if (cartHeader == null)
                {
                    _logger.LogInformation("Sepet bulunamadı. UserId: {UserId}", userId);
                    return Ok(0);
                }

                int cartDetailsCount = await _db.CartDetails.AsNoTracking()
                    .CountAsync(u => u.CartHeaderId == cartHeader.CartHeaderId);

                _logger.LogInformation("GetCount işlemi tamamlandı. UserId: {UserId}, Count: {Count}", userId, cartDetailsCount);
                return Ok(cartDetailsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCount hata verdi. UserId: {UserId}", userId);
                return StatusCode(500, "Bir hata oluştu: " + ex.Message);
            }
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartVM cartVM)
        {
            _logger.LogInformation("CartUpsert çağrıldı. SessionId: {SessionId}, ProductId: {ProductId}",
                cartVM.ShoppingCart.SessionId, cartVM.ShoppingCart.ProductId);

            try
            {
                var cartHeadersFromDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartVM.ShoppingCart.SessionId);

                var products = await _productService.GetProducts();
                var product = products.FirstOrDefault(u => u.ProductId == cartVM.ShoppingCart.ProductId);
                var productExtras = product.ProductExtras;

                List<ShoppingCartExtra> shoppingCartExtras = new List<ShoppingCartExtra>();

                for (int i = 0; i < productExtras.Count; i++)
                {
                    if (cartVM.IsExtraSelected[i])
                    {
                        shoppingCartExtras.Add(new ShoppingCartExtra
                        {
                            ExtraName = productExtras[i].ExtraName,
                            Price = productExtras[i].Price,
                        });
                    }
                }

                if (cartHeadersFromDb == null)
                {
                    CartHeader cartHeader = new()
                    {
                        UserId = cartVM.ShoppingCart.SessionId,
                    };

                    if (cartVM.ShoppingCart.Count <= product.AvailableProducts || !product.Category.ProductCount)
                    {
                        _db.CartHeaders.Add(cartHeader);
                        await _db.SaveChangesAsync();

                        CartDetails cartDetails = new()
                        {
                            ProductId = cartVM.ShoppingCart.ProductId,
                            Count = cartVM.ShoppingCart.Count,
                            ProductExtrasList = shoppingCartExtras,
                            CartHeaderId = cartHeader.CartHeaderId,
                            DetailPrice = CalculateTotalPrice(product.Price, shoppingCartExtras)
                        };

                        _db.CartDetails.Add(cartDetails);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails
                        .Include(u => u.ProductExtrasList)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ProductId == cartVM.ShoppingCart.ProductId &&
                                                  u.CartHeaderId == cartHeadersFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        CartDetails cartDetails = new()
                        {
                            ProductId = cartVM.ShoppingCart.ProductId,
                            Count = cartVM.ShoppingCart.Count,
                            ProductExtrasList = shoppingCartExtras,
                            CartHeaderId = cartHeadersFromDb.CartHeaderId,
                            DetailPrice = CalculateTotalPrice(product.Price, shoppingCartExtras)
                        };

                        _db.CartDetails.Add(cartDetails);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        cartDetailsFromDb.Count += cartVM.ShoppingCart.Count;
                        cartDetailsFromDb.DetailPrice = CalculateTotalPrice(product.Price, cartDetailsFromDb.ProductExtrasList);
                        _db.CartDetails.Update(cartDetailsFromDb);
                        await _db.SaveChangesAsync();
                    }
                }

                int count = await _db.CartDetails.AsNoTracking()
                    .Where(u => u.CartHeaderId == cartHeadersFromDb.CartHeaderId)
                    .CountAsync();

                await _cartHub.Clients.All.SendAsync("ReceiveTableStatusUpdate", count);

                _response.Result = "Everything is done";
                _response.IsSuccess = true;
                _logger.LogInformation("CartUpsert işlemi başarıyla tamamlandı. SessionId: {SessionId}", cartVM.ShoppingCart.SessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CartUpsert hata verdi. SessionId: {SessionId}", cartVM.ShoppingCart.SessionId);
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }
    


    // Fiyatı hesaplayan yardımcı metot
    private double CalculateTotalPrice(double basePrice, List<ShoppingCartExtra> extras)
        {
            double totalPrice = basePrice;

            foreach (var extra in extras)
            {
                totalPrice += extra.Price;
            }

            return totalPrice;
        }





        [HttpPost("PlusCart")]
        public async Task<ResponseDto> PlusCart([FromBody] int cartDetailsId)
        {
            _logger.LogInformation("PlusCart called. CartDetailsId: {CartDetailsId}", cartDetailsId);

            try
            {
                CartDetails cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .First(u => u.CartDetailsId == cartDetailsId);

                cartDetails.Count += 1;
                _db.CartDetails.Update(cartDetails);
                await _db.SaveChangesAsync();

                _response.Result = cartDetails.Count;
                _logger.LogInformation("PlusCart successful. New Count: {Count}, CartDetailsId: {CartDetailsId}", cartDetails.Count, cartDetailsId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PlusCart failed. CartDetailsId: {CartDetailsId}", cartDetailsId);
                _response.Message = e.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("MinusCart")]
        public async Task<ResponseDto> MinusCart([FromBody] int cartDetailsId)
        {
            _logger.LogInformation("MinusCart called. CartDetailsId: {CartDetailsId}", cartDetailsId);

            try
            {
                CartDetails cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .First(u => u.CartDetailsId == cartDetailsId);

                if (cartDetails.Count == 1)
                {
                    int totalCountOfCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                    _db.CartDetails.Remove(cartDetails);

                    if (totalCountOfCartItem == 1)
                    {
                        var cartHeaderToRemove = await _db.CartHeaders
                            .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                        _db.CartHeaders.Remove(cartHeaderToRemove);
                        _logger.LogInformation("CartHeader removed. CartHeaderId: {CartHeaderId}", cartDetails.CartHeaderId);
                    }

                    await _db.SaveChangesAsync();
                    _logger.LogInformation("CartDetails removed. CartDetailsId: {CartDetailsId}", cartDetailsId);
                }
                else
                {
                    cartDetails.Count -= 1;
                    _db.CartDetails.Update(cartDetails);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("MinusCart successful. New Count: {Count}, CartDetailsId: {CartDetailsId}", cartDetails.Count, cartDetailsId);
                }

                _response.Result = cartDetails.Count;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "MinusCart failed. CartDetailsId: {CartDetailsId}", cartDetailsId);
                _response.Message = e.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            _logger.LogInformation("RemoveCart called. CartDetailsId: {CartDetailsId}", cartDetailsId);

            try
            {
                CartDetails cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                _logger.LogInformation("CartDetails removed. CartDetailsId: {CartDetailsId}", cartDetailsId);

                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                    _logger.LogInformation("CartHeader removed. CartHeaderId: {CartHeaderId}", cartDetails.CartHeaderId);
                }

                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _logger.LogInformation("RemoveCart successful. CartDetailsId: {CartDetailsId}", cartDetailsId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveCart failed. CartDetailsId: {CartDetailsId}", cartDetailsId);
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }





        [HttpPost("RemoveShoppingCart")]
        public async Task<ResponseDto> RemoveShoppingCart([FromBody] int cartHeaderId)
        {
            _logger.LogInformation("RemoveShoppingCart called. CartHeaderId: {CartHeaderId}", cartHeaderId);

            try
            {
                CartHeader cartHeader = _db.CartHeaders.First(u => u.CartHeaderId == cartHeaderId);

                _logger.LogInformation("CartHeader found. CartHeaderId: {CartHeaderId}", cartHeaderId);

                // Get related CartDetails and their ProductExtrasList
                var cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .Where(u => u.CartHeaderId == cartHeaderId).ToList();

                _logger.LogInformation("Found {CartDetailsCount} CartDetails for CartHeaderId: {CartHeaderId}", cartDetails.Count, cartHeaderId);

                // Remove related ShoppingCartExtras for each CartDetails entry
                foreach (var detail in cartDetails)
                {
                    if (detail.ProductExtrasList != null && detail.ProductExtrasList.Any())
                    {
                        _db.ShoppingCartExtras.RemoveRange(detail.ProductExtrasList);
                        _logger.LogInformation("Removed {ProductExtrasCount} ShoppingCartExtras for CartDetailsId: {CartDetailsId}", detail.ProductExtrasList.Count, detail.CartDetailsId);
                    }
                }

                // Remove CartDetails
                _db.CartDetails.RemoveRange(cartDetails);
                _logger.LogInformation("Removed {CartDetailsCount} CartDetails for CartHeaderId: {CartHeaderId}", cartDetails.Count, cartHeaderId);

                // Remove CartHeader
                _db.CartHeaders.Remove(cartHeader);
                _logger.LogInformation("Removed CartHeader. CartHeaderId: {CartHeaderId}", cartHeaderId);

                // Save changes
                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
                _logger.LogInformation("ShoppingCart removed successfully. CartHeaderId: {CartHeaderId}", cartHeaderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveShoppingCart failed. CartHeaderId: {CartHeaderId}", cartHeaderId);
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }

    }
}

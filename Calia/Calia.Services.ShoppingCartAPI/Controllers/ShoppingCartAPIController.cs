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
		public ShoppingCartAPIController(AppDbContext db, IMapper mapper, IHubContext<CartHub> cartHub, IProductService productService, IConfiguration configuration)
		{
			_db = db;
			_mapper = mapper;
            _cartHub = cartHub;
			_productService = productService;
			_configuration = configuration;
			_response = new ResponseDto();
		}




		

		[HttpGet("GetCart/{sessionId}")]
		public async Task<ResponseDto> GetCart(string sessionId)
		{
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
                    item.ProductExtrasList = _mapper.Map<List<ShoppingCartExtraDto>>(_db.CartDetails
                    .Include(u => u.ProductExtrasList)
                    .FirstOrDefault(u => u.CartDetailsId == item.CartDetailsId)?.ProductExtrasList);
                    cart.CartHeader.CartTotal += (item.Count * item.DetailPrice);

				}

				

				_response.Result = cart;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;


		}



        [HttpGet("GetCount/{userId}")]
        public async Task<IActionResult> GetCount(string userId)
        {
            try
            {
                // Kullanıcının sepet başlıklarını veritabanından al
                var cartHeader = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                // Eğer sepet başlığı bulunamazsa, 404 döndür
                if (cartHeader == null)
                {
                    return Ok(0);
                }

                // Sepet detaylarını al ve ürün sayısını hesapla
                var cartDetailsCount = await _db.CartDetails
                    .AsNoTracking()
                    .CountAsync(u => u.CartHeaderId == cartHeader.CartHeaderId); // Count direkt sorguda alınabilir

                return Ok(cartDetailsCount); // Sayıyı döndür
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapabilir veya başka bir işlem gerçekleştirebilirsiniz
                return StatusCode(500, "Bir hata oluştu: " + ex.Message); // 500 durumu ile hata mesajı döndür
            }
        }



        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartVM cartVM)
        {
            try
            {
                var cartHeadersFromDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartVM.ShoppingCart.SessionId);

                var products = await _productService.GetProducts();
                var product = products.FirstOrDefault(u => u.ProductId == cartVM.ShoppingCart.ProductId);
                var productExtras = product.ProductExtras;

                List<ShoppingCartExtra> shoppingCartExtras = new List<ShoppingCartExtra>();

                // Ekstraları seçme işlemi
                for (int i = 0; i < productExtras.Count; i++)
                {
                    if (cartVM.IsExtraSelected[i])
                    {
                        var selectedExtra = productExtras[i];
                        ShoppingCartExtra extra = new ShoppingCartExtra()
                        {
                            ExtraName = selectedExtra.ExtraName,
                            Price = selectedExtra.Price,
                        };
                        shoppingCartExtras.Add(extra);
                    }
                }

                // Eğer CartHeader yoksa yeni bir tane oluştur
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

                        CartDetails cartDetails = new CartDetails
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
                        .FirstOrDefaultAsync(u => u.ProductId == cartVM.ShoppingCart.ProductId && u.CartHeaderId == cartHeadersFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        CartDetails cartDetails = new CartDetails
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
                        // Ekstraların aynı olup olmadığını kontrol ediyoruz
                        var existingExtrasNames = cartDetailsFromDb.ProductExtrasList.Select(e => e.ExtraName).ToList();
                        var newExtrasNames = shoppingCartExtras.Select(e => e.ExtraName).ToList();

                        bool areExtrasSame = !existingExtrasNames.Except(newExtrasNames).Any() &&
                                             !newExtrasNames.Except(existingExtrasNames).Any();

                        if (areExtrasSame)
                        {
                            // Ekstralar aynıysa, sadece ürünü güncelle
                            cartDetailsFromDb.Count += cartVM.ShoppingCart.Count;
                            cartDetailsFromDb.DetailPrice = CalculateTotalPrice(product.Price, cartDetailsFromDb.ProductExtrasList); // Fiyatı tekrar hesapla
                            _db.CartDetails.Update(cartDetailsFromDb);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            // Ekstralar farklıysa, yeni CartDetails oluştur
                            CartDetails cartDetails = new CartDetails
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
                    }
                }

                var cartHeadersFromDbId = await _db.CartHeaders.AsNoTracking()
                   .FirstOrDefaultAsync(u => u.UserId == cartVM.ShoppingCart.SessionId);

                var cartDetailsFromDbCount = await _db.CartDetails
                       .AsNoTracking()
                       .Where(u => u.CartHeaderId == cartHeadersFromDbId.CartHeaderId).ToListAsync();
                int Count = cartDetailsFromDbCount.Count();
                await _cartHub.Clients.All.SendAsync("ReceiveTableStatusUpdate", Count);


                _response.Result = "Everything is done";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
            try
            {
                CartDetails cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .First(u => u.CartDetailsId == cartDetailsId);

                cartDetails.Count += 1;
                _db.CartDetails.Update(cartDetails);
                await _db.SaveChangesAsync();

                _response.Result = cartDetails.Count;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("MinusCart")]
        public async Task<ResponseDto> MinusCart([FromBody] int cartDetailsId)
        {
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
                    }

                    await _db.SaveChangesAsync();
                }
                else
                {
                    cartDetails.Count -= 1;
                    _db.CartDetails.Update(cartDetails);
                    await _db.SaveChangesAsync();
                }


                _response.Result = cartDetails.Count;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("RemoveCart")]
		public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
		{
			try
			{
                CartDetails cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
				_db.CartDetails.Remove(cartDetails);
				if (totalCountofCartItem == 1)
				{
					var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
					_db.CartHeaders.Remove(cartHeaderToRemove);

				}
				await _db.SaveChangesAsync();
				_response.IsSuccess = true;


			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}




        [HttpPost("RemoveShoppingCart")]
        public async Task<ResponseDto> RemoveShoppingCart([FromBody] int cartHeaderId)
        {
            try
            {
                CartHeader cartHeader = _db.CartHeaders.First(u => u.CartHeaderId == cartHeaderId);

                // Get related CartDetails and their ProductExtrasList
                var cartDetails = _db.CartDetails.Include(u => u.ProductExtrasList)
                    .Where(u => u.CartHeaderId == cartHeaderId).ToList();

                // Remove related ShoppingCartExtras for each CartDetails entry
                foreach (var detail in cartDetails)
                {
                    if (detail.ProductExtrasList != null && detail.ProductExtrasList.Any())
                    {
                        _db.ShoppingCartExtras.RemoveRange(detail.ProductExtrasList);
                    }
                }

                // Remove CartDetails
                _db.CartDetails.RemoveRange(cartDetails);

                // Remove CartHeader
                _db.CartHeaders.Remove(cartHeader);

                // Save changes
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;



            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}

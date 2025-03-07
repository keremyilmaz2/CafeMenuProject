using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;

namespace Calia.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        public ShoppingCartController(IShoppingCartService shoppingCartService, IOrderService orderService)
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }


        public async Task<IActionResult> CartCaliaIndex()
        {
            string sessionId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = sessionId;
            string? masaNumarasi = HttpContext.Session.GetString("MasaNumarasi");
            ViewBag.MasaNumarasi = masaNumarasi;

            CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

            // Check if CartDetails is empty
            if (cartDto.CartDetails == null || !cartDto.CartDetails.Any())
            {
                
                return RedirectToAction("Menu", "Home"); // Redirect to the desired action
            }

            return View(cartDto);
        }


        //[Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }




        [HttpPost]
        public async Task<IActionResult> SiparisVer(CartDto cartDto)
        {
            string? masaNumarasi = HttpContext.Session.GetString("MasaNumarasi");
            string sessionId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(masaNumarasi))
            {
                ViewBag.MasaNumarasi = masaNumarasi; // .Value gerekmez, string olarak doğrudan kullanılabilir
            }
            else
            {
                ViewBag.MasaNumarasi = "Masa numarası bulunamadı.";
            }

            if (cartDto.CartHeader == null)
            {
                cartDto.CartHeader = new CartHeaderDto();
            }


            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            if ((User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleWaiter))&& !string.IsNullOrEmpty(cartDto.CartHeader.MasaNo))
            {
                cart.CartHeader.MasaNo = cartDto.CartHeader.MasaNo;
            }
            else { 
                cart.CartHeader.MasaNo = masaNumarasi.ToString();
            }
            var response = await _orderService.CreateOrder(cart);
            CartDto responseCartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                var cartResponse = _shoppingCartService.RemoveShoppingCart(responseCartDto.CartHeader.CartHeaderId);
                return RedirectToAction(nameof(Confirmation), new { orderId = responseCartDto.CartHeader.orderId });

            }
            ViewBag.response = response.Message;
            return RedirectToAction(nameof(Confirmation), new { orderId = responseCartDto.CartHeader.orderId });

        }
        [HttpPost]
        public async Task<IActionResult> Ikram(CartDto cartDto)
        {
            string? masaNumarasi = HttpContext.Session.GetString("MasaNumarasi");
            string sessionId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(masaNumarasi))
            {
                ViewBag.MasaNumarasi = masaNumarasi;
            }
            else
            {
                ViewBag.MasaNumarasi = "Masa numarası bulunamadı.";
            }

            if (cartDto.CartHeader == null)
            {
                cartDto.CartHeader = new CartHeaderDto();
            }


            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            if ((User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleWaiter)) && !string.IsNullOrEmpty(cartDto.CartHeader.MasaNo))
            {
                cart.CartHeader.MasaNo = cartDto.CartHeader.MasaNo;
            }
            else
            {
                cart.CartHeader.MasaNo = masaNumarasi.ToString();
            }
            var response = await _orderService.CreateIkram(cart);
            CartDto responseCartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                var cartResponse = _shoppingCartService.RemoveShoppingCart(responseCartDto.CartHeader.CartHeaderId);
                return RedirectToAction(nameof(Confirmation), new { orderId = responseCartDto.CartHeader.orderId });

            }
            return RedirectToAction(nameof(CartIndex));

        }


        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }


        public async Task<IActionResult> RemoveCart(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart Updated Successfully";
                return RedirectToAction(nameof(CartIndex));

            }
            return View();
        }

        public async Task<IActionResult> plus(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.Plus(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart Updated Successfully";
                return RedirectToAction(nameof(CartIndex));

            }
            return View();
        }


        public async Task<IActionResult> minus(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.Minus(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart Updated Successfully";
                return RedirectToAction(nameof(CartIndex));

            }
            return View();
        }




        public async Task<IActionResult> CaliaRemoveCart(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                return Json(new { success = true });

            }
            return View();
        }

        public async Task<IActionResult> Caliaplus(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.Plus(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                int count = JsonConvert.DeserializeObject<int>(Convert.ToString(response.Result));
                return Json(new { success = true, count = count });

            }
            return View();
        }


        public async Task<IActionResult> Caliaminus(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartService.Minus(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                int count = JsonConvert.DeserializeObject<int>(Convert.ToString(response.Result));
                return Json(new { success = true, count = count });

            }
            return View();
        }







        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {

            //var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            string sessionId = HttpContext.Session.GetString("UserId");
            var userId = sessionId;
            ResponseDto? response = await _shoppingCartService.GetCartByUserIdAsync(userId);
            ResponseDto? responsee = await _orderService.GetTables();
            if (response!= null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                var Tables = JsonConvert.DeserializeObject<List<TableNoDto>>(Convert.ToString(responsee.Result));

                cartDto.Tables = Tables.Select(u => new SelectListItem
                {
                    Text = u.MasaNo,
                    Value = u.Id.ToString()
                });
                return cartDto;

            }
            return new CartDto();
        }

    }
}

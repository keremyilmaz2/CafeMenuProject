using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Calia.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService ,ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;        
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponseDTO);
                _tokenProvider.SetToken(loginResponseDTO.Token);
                return RedirectToAction("AdminIndex", "Home");
            }
            else
            {
                
                TempData["error"] = responseDto.Message;
                return View(obj);
            }
                
        }
        [HttpGet]
        public async Task<IActionResult> ShowUsers()
        {
            List<UsersDto>? list = new();

            ResponseDto? response = await _authService.GetAllUsersWithRoles();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UsersDto>>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        public class LockUnlockRequest
        {
            public string UserId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] LockUnlockRequest lockUnlockRequest)
        {
            ResponseDto response = await _authService.LockUnlock(lockUnlockRequest.UserId);

            if (response != null && response.IsSuccess)
            {
                return Ok(response); // Başarılı olduğunda OK döner
            }
            else
            {
                return BadRequest(response?.Message); // Hata durumunda BadRequest döner
            }
        }


        
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
                new SelectListItem{Text=SD.RoleWaiter,Value=SD.RoleWaiter},
            };
            ViewBag.roleList = roleList;

            return View();
        }
        
        [HttpPost]
        public  async Task<IActionResult> Register(RegistrationRequestDTO obj)
        {
            ResponseDto result  =  await _authService.RegisterAsync(obj);
            ResponseDto assignRole;

            if (result!=null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(obj);
                if (assignRole != null && assignRole.IsSuccess) {
                    TempData["success"] = "Registration Successfully";
                    return RedirectToAction("AdminIndex", "Home");
                }
            }

            else
            {
                TempData["error"] = result.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
                new SelectListItem{Text=SD.RoleWaiter,Value=SD.RoleWaiter},
            };
            ViewBag.roleList = roleList;

            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);


            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));   



            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}

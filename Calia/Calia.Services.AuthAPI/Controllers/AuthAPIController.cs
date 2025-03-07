using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Models;
using Calia.Services.AuthAPI.Models.Dto;
using Calia.Services.AuthAPI.Models.Dto;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService ,IConfiguration configuration,AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            
            _configuration = configuration;
            _userManager = userManager;
            _authService = authService;
            _db = db;
            _responseDto = new();
        }


        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model )
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);

            }
            return Ok(_responseDto);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            // Kullanıcıyı veritabanından al
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == model.UserName);

            // Kullanıcı yoksa ya da kilitlenmişse
            if (user == null || (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Kullanıcı kilitli veya geçersiz kullanıcı adı/şifre.";
                return BadRequest(_responseDto);
            }

            // Eğer kullanıcı mevcutsa, login işlemine devam et
            var loginResponse = await _authService.Login(model);

            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Kullanıcı adı veya şifre yanlış.";
                return BadRequest(_responseDto);
            }

            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            var assignRoleSuccessfull = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignRoleSuccessfull)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpGet("GetWaiter")]
        public async Task<IActionResult> GetWaiter()
        {
            // Waiter rolüne sahip kullanıcıları getir
            var waiters = await _userManager.GetUsersInRoleAsync("waiter");

            // Dönüş için uygun bir DTO kullanarak verileri hazırlayın
            var waiterDtos = waiters.Select(w => new
            {
                w.Id,
                w.Name,
                w.Email
            }).ToList();

            return Ok(waiterDtos); 
        }

        [HttpPost("LockUnlock/{UserId}")]
        public async Task<IActionResult> LockUnlock(string UserId)
        {
            var objFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == UserId);
            if (objFromDb == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error while Locking/Unlocking";
                return BadRequest(_responseDto);
            }

            // Kullanıcının kilit durumunu değiştir
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // Eğer kullanıcı kilitli ise kilidi aç
                objFromDb.LockoutEnd = null; // Kilidi açmak için LockoutEnd'i null yap
            }
            else
            {
                // Kullanıcı kilitli değilse kilitle
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100); // Gelecek bir tarihe ayarla
            }

            _db.ApplicationUsers.Update(objFromDb);
            await _db.SaveChangesAsync(); // Asenkron kaydetme
            _responseDto.IsSuccess = true;
            _responseDto.Message = "İşlem Tamam"; // Kullanıcı dostu mesaj
            return Ok(_responseDto); // Başarı durumunda döndür
        }


        [HttpGet("GetAllUsersWithRoles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            // Tüm kullanıcıları getir
            var users = _userManager.Users.ToList();

            // Her kullanıcı için rollerini al ve uygun DTO ile dön
            var userDtos = new List<UsersDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Kullanıcının rollerini al

                userDtos.Add(new UsersDto
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList(), // Roller listesi olarak eklenir
                    Lock_Unlock =user.LockoutEnd != null && user.LockoutEnd > DateTime.Now
                });
            }
            _responseDto.IsSuccess = true;
            _responseDto.Result = userDtos;

            return Ok(_responseDto);
        }


    }
}

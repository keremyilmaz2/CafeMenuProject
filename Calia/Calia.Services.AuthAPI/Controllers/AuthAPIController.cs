using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Models;
using Calia.Services.AuthAPI.Models.Dto;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        private readonly ILogger<AuthAPIController> _logger;
        protected ResponseDto _responseDto;

        public AuthAPIController(
            IAuthService authService,
            IConfiguration configuration,
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthAPIController> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _authService = authService;
            _db = db;
            _logger = logger;
            _responseDto = new();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            _logger.LogInformation("[AUTH API] Yeni kullanıcı kaydı başlatıldı: {@Model}", model);

            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _logger.LogWarning("[AUTH API] Kullanıcı kaydı başarısız: {ErrorMessage}", errorMessage);
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }

            _logger.LogInformation("[AUTH API] Kullanıcı başarıyla kaydedildi: {@Model}", model);
            return Ok(_responseDto);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            _logger.LogInformation("[AUTH API] Login işlemi başlatıldı: {@Model}", model);

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == model.UserName);
            if (user == null || (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now))
            {
                _logger.LogWarning("[AUTH API] Kullanıcı kilitli veya geçersiz giriş: {UserName}", model.UserName);
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Kullanıcı kilitli veya geçersiz kullanıcı adı/şifre.";
                return BadRequest(_responseDto);
            }

            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _logger.LogWarning("[AUTH API] Geçersiz kullanıcı adı veya şifre: {UserName}", model.UserName);
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Kullanıcı adı veya şifre yanlış.";
                return BadRequest(_responseDto);
            }

            _logger.LogInformation("[AUTH API] Kullanıcı başarılı giriş yaptı: {UserName}", model.UserName);
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            _logger.LogInformation("[AUTH API] Kullanıcıya rol atanıyor: {Email} - {Role}", model.Email, model.Role);

            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _logger.LogError("[AUTH API] Rol atama başarısız: {Email} - {Role}", model.Email, model.Role);
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered";
                return BadRequest(_responseDto);
            }

            _logger.LogInformation("[AUTH API] Kullanıcıya rol başarıyla atandı: {Email} - {Role}", model.Email, model.Role);
            return Ok(_responseDto);
        }

        [HttpGet("GetWaiter")]
        public async Task<IActionResult> GetWaiter()
        {
            _logger.LogInformation("[AUTH API] Tüm garsonlar getiriliyor...");
            var waiters = await _userManager.GetUsersInRoleAsync("waiter");

            var waiterDtos = waiters.Select(w => new
            {
                w.Id,
                w.Name,
                w.Email
            }).ToList();

            _logger.LogInformation("[AUTH API] {Count} garson bulundu.", waiterDtos.Count);
            return Ok(waiterDtos);
        }
    }
}

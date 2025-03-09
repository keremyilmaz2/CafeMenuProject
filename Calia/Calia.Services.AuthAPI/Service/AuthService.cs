using Calia.Services.AuthAPI.Data;
using Calia.Services.AuthAPI.Models;
using Calia.Services.AuthAPI.Models.Dto;
using Calia.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Calia.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger; // ✅ Logger eklendi

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthService> logger) // ✅ Logger dependency injection ile alındı
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger; // ✅ Logger tanımlandı
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                _logger.LogWarning("AssignRole: Kullanıcı bulunamadı. Email: {Email}", email);
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogInformation("AssignRole: Rol bulunamadı, yeni rol oluşturuluyor: {Role}", roleName);
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);
            _logger.LogInformation("AssignRole: Kullanıcıya rol atandı. Email: {Email}, Role: {Role}", email, roleName);

            return true;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Login: Kullanıcı bulunamadı. Kullanıcı Adı: {UserName}", loginRequestDTO.UserName);
                return new LoginResponseDto() { User = null, Token = "" };
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (!isValid)
            {
                _logger.LogWarning("Login: Geçersiz şifre. Kullanıcı Adı: {UserName}", loginRequestDTO.UserName);
                return new LoginResponseDto() { User = null, Token = "" };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            _logger.LogInformation("Login: Kullanıcı başarıyla giriş yaptı. Kullanıcı Adı: {UserName}", loginRequestDTO.UserName);

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto()
            {
                User = userDto,
                Token = token,
            };
        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Register: Kullanıcı başarıyla oluşturuldu. Email: {Email}", registrationRequestDTO.Email);
                    return "";
                }
                else
                {
                    string error = result.Errors.FirstOrDefault()?.Description ?? "Bilinmeyen hata";
                    _logger.LogError("Register: Kullanıcı oluşturma başarısız. Email: {Email}, Hata: {Error}", registrationRequestDTO.Email, error);
                    return error;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register: Beklenmeyen hata oluştu. Email: {Email}", registrationRequestDTO.Email);
                return "Sunucu hatası, lütfen tekrar deneyiniz.";
            }
        }
    }
}

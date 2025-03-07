using Calia.Services.AuthAPI.Models.Dto;
using Calia.Services.AuthAPI.Models.Dto;

namespace Calia.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO);
        Task<string> Register(RegistrationRequestDTO registrationRequestDTO);

        Task<bool> AssignRole(string email, string roleName);
    }
}

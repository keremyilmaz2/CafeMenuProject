using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<ResponseDto?> GetAllUsersWithRoles();
        Task<ResponseDto?> LockUnlock(string UserId);
    }
}

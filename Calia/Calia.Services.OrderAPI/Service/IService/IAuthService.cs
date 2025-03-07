using Calia.Services.OrderAPI.Models.Dto;

namespace Calia.Services.OrderAPI.Service.IService
{
    public interface IAuthService
    {
        Task<IEnumerable<ApplicationUserDto>> GetWaitersAsync();
    }
}

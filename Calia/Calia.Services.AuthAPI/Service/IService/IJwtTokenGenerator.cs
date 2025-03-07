using Calia.Services.AuthAPI.Models;

namespace Calia.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles);

    }
}

using Calia.Services.OrderAPI.Models.Dto;
using Calia.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;

namespace Calia.Services.OrderAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetWaitersAsync()
        {
            var client = _httpClientFactory.CreateClient("Auth");
            var response = await client.GetAsync($"/api/auth/GetWaiter");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ApplicationUserDto>>(Convert.ToString(resp.Result));
            }
            return new List<ApplicationUserDto>();
        }
    }
}

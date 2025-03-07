using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto,bool withBearer = true );
        

    }
}

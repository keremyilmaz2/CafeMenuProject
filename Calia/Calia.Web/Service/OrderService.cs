using Calia.Web.Models;
using Calia.Web.Service.IService;
using Calia.Web.Utility;

namespace Calia.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }


        public async Task<ResponseDto?> CreateKisiselGider(KisiselGiderDto kisiselGiderDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = kisiselGiderDto,
                Url = SD.OrderAPIAPIBase + "/api/order/CreateKisiselGider"
            });
        }

        public async Task<ResponseDto?> CancelOrder(PaymentVm PaymentVm)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = PaymentVm,
                Url = SD.OrderAPIAPIBase + "/api/order/CancelOrder"
            });
        }


        public async Task<ResponseDto?> GetAdmins()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetAdmins"
            });
        }

        public async Task<ResponseDto?> GetKisiselGiders(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/CreateKisiselGider/" + id
            });
        }

        public async Task<ResponseDto?> AddTables(string masa)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderAPIAPIBase + "/api/order/AddTable/" + masa
            });
        }

        public async Task<ResponseDto?> DeleteTable(int masaId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.OrderAPIAPIBase + "/api/order/DeleteTable/" + masaId
            });
        }


        public async Task<ResponseDto?> ChangeTable(ChangeTableRequest changeTableRequest)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = changeTableRequest,
                Url = SD.OrderAPIAPIBase + "/api/order/ChangeTable"
            });
        }

        public async Task<ResponseDto?> ChnagePrinter(string PrinterId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.POST,
				Data = PrinterId,
				Url = SD.OrderAPIAPIBase + "/api/order/ChangePrinter"
			});
		}

        public async Task<ResponseDto?> CloseTable(TableCloseRequest tableCloseRequest)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = tableCloseRequest,
                Url = SD.OrderAPIAPIBase + "/api/order/CloseTable"
            });
        }

        public async Task<ResponseDto?> CreateIkram(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIAPIBase + "/api/order/Ikram"
            });
        }

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIAPIBase + "/api/order/SiparisVer"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = SD.OrderAPIAPIBase + "/api/order/CreateStripeSession"
            });
        }


		public async Task<ResponseDto?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetOrders/" + userId
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetOrder/" + orderId
            });
        }

		public async Task<ResponseDto?> GetPrinters()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = SD.ApiType.GET,
				Url = SD.OrderAPIAPIBase + "/api/order/GetPrinters"
			});
		}

        public async Task<ResponseDto?> GetTableDetails(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetTableDetails/" + id
            });
        }

        public async Task<ResponseDto?> GetTables()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetTables"
            });
        }

        public async Task<ResponseDto?> OdemeAlForUser(OrderHeaderDto orderHeaderDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = orderHeaderDto,
                Url = SD.OrderAPIAPIBase + "/api/order/TakePayment"
            });

        }

        public async Task<ResponseDto?> ProcessPayment(PaymentVm PaymentVm)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = PaymentVm,
                Url = SD.OrderAPIAPIBase + "/api/order/ProcessPayment"
            });
        }
        
        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPIAPIBase + "/api/order/UpdateOrderStatus/"+orderId
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.OrderAPIAPIBase + "/api/order/ValidateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetRapor()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetRapor"
            });
        }

        public async Task<ResponseDto?> TumunuGor()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/TumunuGor"
            });
        }

        public async Task<ResponseDto?> GetGunSonu()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetGunSonu"
            });
        }

        public async Task<ResponseDto?> GetTablewithId(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIAPIBase + "/api/order/GetTablewithId/" + id
            });
        }

        public async Task<ResponseDto?> GunSonuPost(GunSonuDto gunSonuDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = gunSonuDto,
                Url = SD.OrderAPIAPIBase + "/api/order/GunSonuPost"
            });
        }
    }
}

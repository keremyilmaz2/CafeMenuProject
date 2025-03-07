using Calia.Web.Models;

namespace Calia.Web.Service.IService
{
    public interface IOrderService  
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> OdemeAlForUser(OrderHeaderDto orderHeaderDto);
        Task<ResponseDto?> CreateIkram(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto?> GetOrder(int orderId);
        Task<ResponseDto?> GetTables();
        Task<ResponseDto?> AddTables(string masa);
        Task<ResponseDto?> DeleteTable(int masaId);
        Task<ResponseDto?> GetTableDetails(int id);
        Task<ResponseDto?> GetTablewithId(int id);
        Task<ResponseDto?> GetPrinters();
		Task<ResponseDto?> ChnagePrinter(string PrinterId);
		Task<ResponseDto?> UpdateOrderStatus(int orderId,string newStatus);
        Task<ResponseDto?> CloseTable(TableCloseRequest tableCloseRequest);
        
        Task<ResponseDto?> ChangeTable(ChangeTableRequest changeTableRequest);
        Task<ResponseDto?> ProcessPayment(PaymentVm PaymentVm);
        Task<ResponseDto?> CancelOrder(PaymentVm PaymentVm);
        Task<ResponseDto?> GunSonuPost(GunSonuDto gunSonuDto);
        
        Task<ResponseDto?> CreateKisiselGider(KisiselGiderDto kisiselGiderDto);
        Task<ResponseDto?> GetAdmins();
        Task<ResponseDto?> GetKisiselGiders(int id);

        Task<ResponseDto?> GetRapor();
        Task<ResponseDto?> TumunuGor();
        Task<ResponseDto?> GetGunSonu();
    }
}

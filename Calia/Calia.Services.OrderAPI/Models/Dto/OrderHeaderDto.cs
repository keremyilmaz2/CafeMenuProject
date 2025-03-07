using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models.Dto
{
    public class OrderHeaderDto
    {
        
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public double OrderTotal { get; set; }
        public string? MasaNo { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public string? PaymentStatus { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }
        public IEnumerable<CancelDetailsDto> CancelDetails { get; set; }
        public double? Ikram { get; set; }
        public double? Iskonto { get; set; }
        public double? Nakit { get; set; }
        public double? KrediKarti { get; set; }
        public string? personTakingOrder { get; set; }
    }
}

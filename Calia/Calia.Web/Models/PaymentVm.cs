using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Calia.Web.Models
{
    public class PaymentVm
    {
        [ValidateNever]
        public TableDetailsDto TableDetails { get; set; }
        [ValidateNever]
        public List<OrderDetailsDto> UnpaidOrderDetails { get; set; }
        [ValidateNever]
        public List<OrderDetailsDto> PaidOrderDetails { get; set; }

        [ValidateNever]
        public List<OrderDetailsDto> IkramOrderDetails { get; set; }
        [ValidateNever]
        public string SelectedOrderDetailsIds { get; set; }
        [ValidateNever]
        public string SelectedOrderDetailCounts { get; set; }

        // Ödeme alanları
        [ValidateNever]
        public int TableId { get; set; }
        public double? CashAmount { get; set; }
        public double? CreditCardAmount { get; set; }
        public double? IkramAmount { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? Tables { get; set; }
        [ValidateNever]
        public int GecilmekIstenilenMasa { get; set; }
        public string? TypeOfPayment { get; set; }

    }
}

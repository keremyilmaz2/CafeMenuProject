using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Calia.Services.OrderAPI.Models.Dto
{
    public class KisiselGiderVm
    {
        [ValidateNever]
        public KisiselGiderDto KisiselGider { get; set; } = new KisiselGiderDto();
        [ValidateNever]
        public int AdminId { get; set; }
        public double? CashAmount { get; set; }
        public string Reason { get; set; } // Sebep eklendi
        [ValidateNever]
        public IEnumerable<SelectListItem>? Admins { get; set; }
    }
}

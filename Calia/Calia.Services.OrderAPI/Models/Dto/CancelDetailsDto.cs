using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models.Dto
{
    public class CancelDetailsDto
    {
        
        public int CancelDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public List<OrderExtraDto>? ProductExtrasList { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CancelTime { get; set; }
        public bool? isPaid { get; set; } = false;
        public string MasaNo { get; set; }

    }
}

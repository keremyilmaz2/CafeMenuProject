using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Models
{
    public class CancelDetailsDto
    {
        [Key]
        public int CancelDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeaderDto? OrderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        [ValidateNever]
        public List<OrderExtraDto>? ProductExtrasList { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CancelTime { get; set; }
        public bool? isPaid { get; set; } = false;
        public string MasaNo { get; set; }

    }
}

using Calia.Services.OrderAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public int? OdemesiAlinmisCount { get; set; } = 0;
        public string ProductName { get; set; }
        public double Price { get; set; }
        [ValidateNever]
		public List<OrderExtra>? ProductExtrasList { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CancelTime { get; set; }
        public bool? isPaid { get; set; } = false;

    }
}

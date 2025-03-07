using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ProductAPI.Models.Dto
{
    public class OrderDetailsDto
    {

        public int? OrderDetailsId { get; set; }
        public int? OrderHeaderId { get; set; }
        public int? ProductId { get; set; }

        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public int? OdemesiAlinmisCount { get; set; }
        public string? ProductName { get; set; }
        public double Price { get; set; }
        public List<OrderExtraDto>? ProductExtrasList { get; set; }

        public string? PaymentStatus { get; set; }
        public bool? isPaid { get; set; }
        //sonradan
        public string? MasaNo { get; set; }

        public DateTime? CancelTime { get; set; }
    }
}

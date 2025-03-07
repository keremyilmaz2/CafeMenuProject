using Calia.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.OrderAPI.Models
{
    public class TableDetails
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        [ForeignKey("TableId")]
        public TableNo? tableNo { get; set; }
        [ValidateNever]
        public List<OrderHeader>? OrderHeaders { get; set; }
        public string? PaymentStatus { get; set; }
        public double? Ikram { get; set; }
        public double? Iskonto { get; set; }
        public double? Nakit { get; set; }
        public double? KrediKarti { get; set; }
        public double? TotalTable { get; set; } = 0;
        public double? AlinanFiyat { get; set; } = 0;
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public bool? isClosed { get; set; }
    }
}

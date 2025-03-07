
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Web.Models
{
    public class TableDetailsDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        [ForeignKey("TableId")]
        public TableNoDto? tableNo { get; set; }
        public List<OrderHeaderDto>? OrderHeaders { get; set; }
        public string? PaymentStatus { get; set; }
        public double? Ikram { get; set; }
        public double? Iskonto { get; set; }
        public double? Nakit { get; set; }
        public double? KrediKarti { get; set; }
        public double? TotalTable { get; set; }
        public double? AlinanFiyat { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public bool? isClosed { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.OrderAPI.Models
{
    public class KisiselGider
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        [ForeignKey("AdminId")]
        public AdminNames? AdminName { get; set; }
        public double? AlinanPara { get; set; }
        public string Reason { get; set; } // Sebep eklendi
        public DateTime? GiderTarihi { get; set; }
    }
}

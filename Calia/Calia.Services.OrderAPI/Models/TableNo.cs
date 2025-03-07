using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models
{
    public class TableNo
    {
        [Key]
        public int Id { get; set; }
        public string MasaNo { get; set; }
        public bool IsOccupied { get; set; } = false;
    }
}

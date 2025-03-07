using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models
{
    public class OrderExtra
    {
        [Key]
        public int Id { get; set; }
        public string ExtraName { get; set; }
        public double Price { get; set; }
    }
}

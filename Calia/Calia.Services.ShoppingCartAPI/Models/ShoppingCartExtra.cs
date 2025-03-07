using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ShoppingCartAPI.Models
{
    public class ShoppingCartExtra
    {
        [Key]
        public int Id { get; set; }
        public string ExtraName { get; set; }
        public double Price { get; set; }
    }
}

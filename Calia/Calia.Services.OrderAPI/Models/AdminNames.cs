using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models
{
    public class AdminNames
    {
        [Key]
        public int Id { get; set; }
        public string? AdminName { get; set; }
    }
}

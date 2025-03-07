using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models.Dto
{
    public class OrderExtraDto
    {
        public int Id { get; set; }
        public string ExtraName { get; set; }
        public double Price { get; set; }
    }
}

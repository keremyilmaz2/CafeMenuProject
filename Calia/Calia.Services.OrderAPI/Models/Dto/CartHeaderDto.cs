using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Calia.Services.OrderAPI.Models.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public double CartTotal { get; set; }
        public string? MasaNo { get; set; }
        [ValidateNever]
        public int? orderId { get; set; }

    }
}

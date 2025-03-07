using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ShoppingCartAPI.Models.Dto
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public ProductDto? Product { get; set; }
        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }
        [ValidateNever]
        public List<ProductExtraDto> SelectedExtra { get; set; }

        [NotMapped]
        public double Price { get; set; }
        public string SessionId { get; set; }
    }
}

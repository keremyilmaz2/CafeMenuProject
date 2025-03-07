using Calia.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {

        [Key]
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto Product { get; set; }
        public int Count { get; set; }
        public double DetailPrice { get; set; }
        public List<ShoppingCartExtra> ProductExtrasList { get; set; }

	}
}

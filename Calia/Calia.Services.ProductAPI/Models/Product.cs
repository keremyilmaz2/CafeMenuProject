using Calia.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.ProductAPI.Models
{
    public class Product
    {
		[Key]
		public int ProductId { get; set; }
		[Required]
		public string Name { get; set; }
		[Range(1, 1000)]
		public double Price { get; set; }
		public int CategoryId { get; set; }
		[ValidateNever]
		public int AvailableProducts { get; set; } = 0;
		[ValidateNever]
		public List<ProductMaterial>? ProductMaterials { get; set; }

		[ValidateNever]
		public List<ProductExtra>? ProductExtras { get; set; }


	}
}

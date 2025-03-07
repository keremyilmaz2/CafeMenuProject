using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ProductAPI.Models
{
	public class ProductMaterial
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string MaterialName { get; set; }
		[Required]
		public double Amount { get; set; }
		public int ProductId { get; set; }
		//[ForeignKey("ProductId")]
		//[ValidateNever]
		//public Product Product { get; set; }
	}
}
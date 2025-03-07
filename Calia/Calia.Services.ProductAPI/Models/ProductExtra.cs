using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ProductAPI.Models
{
	public class ProductExtra
	{

		[Key]
		public int Id { get; set; }
		[Required]
		public string ExtraName { get; set; }
		[Required]
		public double Price { get; set; }
		public int ProductId { get; set; }
		//[ForeignKey("ProductId")]
		//[ValidateNever]
		//public Product Product { get; set; }
	}
}
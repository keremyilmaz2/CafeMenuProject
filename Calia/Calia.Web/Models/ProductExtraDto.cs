using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Models
{
	public class ProductExtraDto
	{
		public int Id { get; set; }
		public string ExtraName { get; set; }
		public double Price { get; set; }
		public int ProductId { get; set; }
		public ProductDto? Product { get; set; }
	}
}
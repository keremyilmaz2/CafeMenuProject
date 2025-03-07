using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ShoppingCartAPI.Models.Dto
{
	public class ProductExtraDto
	{
		public int Id { get; set; }
		public string ExtraName { get; set; }
		public double Price { get; set; }
		public int ProductId { get; set; }
	}
}
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ProductAPI.Models.Dto
{
	public class ProductMaterialDto
	{
		
		public int Id { get; set; }
		public string MaterialName { get; set; }
		public double Amount { get; set; }
		public int ProductId { get; set; }
		//public Product? Product { get; set; }
	}
}
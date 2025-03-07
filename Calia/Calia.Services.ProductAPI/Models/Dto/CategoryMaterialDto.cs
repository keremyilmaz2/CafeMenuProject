using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.ProductAPI.Models.Dto
{
	public class CategoryMaterialDto
	{
		public int MaterialId { get; set; }
		public string MaterialName { get; set; }
		public int CategoryId { get; set; }
		public CategoryDto Category { get; set; }
	}
}

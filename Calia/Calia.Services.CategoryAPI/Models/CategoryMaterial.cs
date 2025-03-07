using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.CategoryAPI.Models
{
	public class CategoryMaterial
	{
		[Key]
		public int MaterialId { get; set; }
		public string MaterialName { get; set; }
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		public Category? Category { get; set; }
	}
}

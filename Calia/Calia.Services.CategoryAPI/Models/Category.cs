using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.CategoryAPI.Models
{
    public class Category
    {
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(30)]
		[DisplayName("Category Name")]
		public string Name { get; set; }
		[DisplayName("Display Order")]
		[Range(0, 100)]
		public int DisplayOrder { get; set; }

		[ValidateNever]
		public bool ProductCount { get; set; }
		[ValidateNever]
		public List<CategoryMaterial>? CategoryMaterials { get; set; }

		[ValidateNever]
		public List<CategoryExtra>? CategoryExtras { get; set; }



	}
}

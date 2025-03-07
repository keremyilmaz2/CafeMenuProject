using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Models
{
    public class CategoryDto
    {

		
		public int Id { get; set; }
		public string Name { get; set; }
		public int DisplayOrder { get; set; }
		public bool ProductCount { get; set; }
		public List<CategoryMaterialDto>? CategoryMaterials { get; set; }
		public List<CategoryExtraDto>? CategoryExtras { get; set; }
	}
}

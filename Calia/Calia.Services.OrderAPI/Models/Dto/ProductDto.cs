using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.OrderAPI.Models.Dto
{
	public class ProductDto
	{

		public int ProductId { get; set; }

		public string Name { get; set; }

		public double Price { get; set; }
		public int CategoryId { get; set; }
		public CategoryDto? Category { get; set; }
        public int Count { get; set; }
        public int AvailableProducts { get; set; }
		public List<ProductMaterialDto>? ProductMaterials { get; set; }
		public List<ProductExtraDto>? ProductExtras { get; set; }


	}
}

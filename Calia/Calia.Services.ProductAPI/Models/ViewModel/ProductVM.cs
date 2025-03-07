using Calia.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Calia.Services.ProductAPI.Models.ViewModel
{
	public class ProductVM
	{
		public ProductDto Product { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryMaterials { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryExtras { get; set; }
		[ValidateNever]
		public List<ProductDto> ProductList { get; set; }
		[ValidateNever]
		public List<bool> IsExtraSelected { get; set; }
	}
}

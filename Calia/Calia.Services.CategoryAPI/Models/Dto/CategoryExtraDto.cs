using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.CategoryAPI.Models.Dto
{
	public class CategoryExtraDto
	{
		public int ExtraId { get; set; }
		public string ExtraName { get; set; }
		public double ExtraPrice { get; set; }
		public int CategoryId { get; set; }
	}
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Web.Models
{
	public class StockMaterialDto
	{
		public int MaterialId { get; set; }
		public string MaterialName { get; set; }
		public double MaterialAmount { get; set; }
	}
}

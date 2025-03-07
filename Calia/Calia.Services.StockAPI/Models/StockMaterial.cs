using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calia.Services.StockAPI.Models
{
	public class StockMaterial
	{
		[Key]
		public int MaterialId { get; set; }
		public string MaterialName { get; set; }
		public double MaterialAmount { get; set; } = 0;
	}
}

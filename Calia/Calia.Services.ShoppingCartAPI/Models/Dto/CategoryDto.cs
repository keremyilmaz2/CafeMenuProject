
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calia.Services.ShoppingCartAPI.Models.Dto
{
    public class CategoryDto
    {

		
		public int Id { get; set; }
		public string Name { get; set; }
        
        public bool ProductCount { get; set; }
    }
}

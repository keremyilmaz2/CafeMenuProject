using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Calia.Services.ShoppingCartAPI.Models.Dto
{
    public class CartVM
    {
        public ShoppingCart? ShoppingCart { get; set; }
        [ValidateNever]
        public List<bool> IsExtraSelected { get; set; }
        //[ValidateNever]
        //public List<CategoryVM> categoryVMs { get; set; }
    }
}

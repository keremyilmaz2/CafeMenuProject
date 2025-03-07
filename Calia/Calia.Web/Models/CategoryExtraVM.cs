
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Calia.Web.Models
{
    public class CategoryExtraVM
    {
        public CategoryExtraDto CategoryExtra { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ExtraNameList { get; set; }
        public List<bool> IsCategorySelected { get; set; }


    }
}

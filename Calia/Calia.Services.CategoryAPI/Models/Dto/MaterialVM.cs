
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Calia.Services.CategoryAPI.Models.Dto
{
    public class MaterialVM
    {
        public string MaterialName { get; set; }
        public List<string> Categories { get; set; }
    }
}

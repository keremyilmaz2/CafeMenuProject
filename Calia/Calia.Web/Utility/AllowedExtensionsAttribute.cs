using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Utility
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extentions;
        public AllowedExtensionsAttribute(string[] extentions)
        {
            _extentions = extentions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null) {
                var extension = Path.GetExtension(file.FileName);
                if (!_extentions.Contains(extension.ToLower()))
                {
                    return new ValidationResult("This Photo extension is not allowed!");

                }
            } 

            return ValidationResult.Success;
        }
    }
}

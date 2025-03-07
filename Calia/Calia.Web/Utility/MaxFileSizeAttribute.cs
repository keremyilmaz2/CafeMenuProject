using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Utility
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxfilesize;
        public MaxFileSizeAttribute(int maxfilesize)
        {
            _maxfilesize = maxfilesize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                
                if (file.Length>(_maxfilesize*1024*1024))
                {
                    return new ValidationResult($"Maximum allowed file size is {_maxfilesize} MB.");

                }
            }

            return ValidationResult.Success;
        }
    }
}

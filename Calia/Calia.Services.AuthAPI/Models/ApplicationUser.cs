using Microsoft.AspNetCore.Identity;

namespace Calia.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name {  get; set; }

    }
}

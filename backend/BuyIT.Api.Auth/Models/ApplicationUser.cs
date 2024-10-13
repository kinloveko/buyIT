using Microsoft.AspNetCore.Identity;

namespace BuyIT.Api.Auth.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}

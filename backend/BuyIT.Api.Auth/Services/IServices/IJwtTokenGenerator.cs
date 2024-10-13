using BuyIT.Api.Auth.Models;

namespace BuyIT.Api.Auth.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user,IEnumerable<string> roles);
    }
}

using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace BuyIT.Api.Gateway.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService.AuthServiceClient _authServiceClient;

        public AuthController(AuthService.AuthServiceClient authServiceClient)
        {
            _authServiceClient = authServiceClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            var response = await _authServiceClient.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logs in a user", Description = "Authenticate a user with username and password.\nExample:\n\n\n```json\n{ \n\"email\": \"admin@gmail.com\",\n\"password\":\"Admin123!\"\n}\n```")]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authServiceClient.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("assign-role")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var response = await _authServiceClient.AssignRoleAsync(request);
            return Ok(response);
        }
    }
}
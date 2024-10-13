using BuyIT.Api.Auth.Models.Dto;
using BuyIT.Api.Auth.Services.IServices;
using BuyIT.API.Auth.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BuyIT.Api.Auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

      
        public AuthApiController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _response = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {

            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
           // await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            _response.Message = "User created successfully!";
            return Ok(_response);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logs in a user", Description = "Authenticate a user with username and password.\nExample:\n\n\n```json\n{ \n\"userName\": \"admin@gmail.com\",\n\"password\":\"Admin123!\"\n}\n```")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);

        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            return Ok(_response);

        }
    }
}

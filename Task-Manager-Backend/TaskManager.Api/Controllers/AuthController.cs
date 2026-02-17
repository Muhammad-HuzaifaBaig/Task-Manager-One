using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.Services.Auth;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpGet("test-exception")]
        public IActionResult TestException()
        {
            throw new Exception("This is a test exception to verify global middleware");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            _logger.LogInformation("Login request received");
            //try
            //{
                if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    _logger.LogWarning("Login validation failed: Missing username or password");
                    var errorResponse = ResponseMessage.BadRequest("Invalid login request.");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _authService.Login(request);
                _logger.LogInformation("Login completed with status code {StatusCode}", response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error occurred while creating user");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDTO request)
        {
            _logger.LogInformation("Signup request received");
            //try
            //{
                if (request == null || 
                    string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.Password) || 
                    string.IsNullOrWhiteSpace(request.FullName))
                {
                    _logger.LogWarning("Signup validation failed: Missing required fields");
                    var errorResponse = ResponseMessage.BadRequest("All fields are required.");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _authService.Signup(request);
                _logger.LogInformation("Signup completed with status code {StatusCode}", response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error occurred while creating user");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }
    }
}

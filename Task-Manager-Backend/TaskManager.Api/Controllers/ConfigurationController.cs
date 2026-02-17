using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.Services.Auth;
using TaskManager.Business.Services.Configuration;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<ConfigurationController> _logger;
        public ConfigurationController(IConfigurationService configurationService, ILogger<ConfigurationController> logger)
        {
            _configurationService = configurationService;
            _logger = logger;
        }

        [HttpGet("priorities")]
        public async Task<IActionResult> GetAllPriority()
        {
            _logger.LogInformation("GetAllPriority request received.");
            try
            {
                var response = await _configurationService.GetAllPrioritiesAsync();
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
                return StatusCode(errorResponse.statusCode, errorResponse);
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetAllStatuses()
        {
            _logger.LogInformation("GetAllStatuses request received.");
            try
            {
                var response = await _configurationService.GetAllStatusesAsync();
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
                return StatusCode(errorResponse.statusCode, errorResponse);
            }
        }

    }
}

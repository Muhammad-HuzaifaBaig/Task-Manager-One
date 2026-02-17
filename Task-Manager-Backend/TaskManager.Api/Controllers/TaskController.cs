using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using TaskManager.Business.Services.Task;
using TaskManager.Model.Enum;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }
        

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDTO request)
        {
            _logger.LogInformation("CreateTask request received");
            //try
            //{
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleIdClaim))
                {
                    var errorResponse = ResponseMessage.Unauthorized("Invalid token claims");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                int currentUserId = int.Parse(userIdClaim);
                int currentUserRoleId = int.Parse(roleIdClaim);

                if (request == null || string.IsNullOrWhiteSpace(request.TaskTitle) || request.AssignedUserId <= 0)
                {
                    _logger.LogWarning("CreateTask validation failed");
                    var errorResponse = ResponseMessage.BadRequest("Invalid task data. Title and assigned user are required.");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _taskService.CreateTaskAsync(request, currentUserId, currentUserRoleId);
                _logger.LogInformation("CreateTask completed with status code {StatusCode}", response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in CreateTask");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            _logger.LogInformation("GetTaskById request received for TaskId {TaskId}", taskId);
            //try
            //{
                if (taskId <= 0)
                {
                    _logger.LogWarning("GetTaskById validation failed");
                    var errorResponse = ResponseMessage.BadRequest("Valid task ID is required");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _taskService.GetTaskByIdAsync(taskId);
                _logger.LogInformation("GetTaskById completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in GetTaskById");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks(int? status)
        {
            _logger.LogInformation("GetMyTasks request received");
            //try
            //{
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    var errorResponse = ResponseMessage.Unauthorized("Invalid token");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                int currentUserId = int.Parse(userIdClaim);

                var response = await _taskService.GetMyTasksAsync(currentUserId, status);
                _logger.LogInformation("GetMyTasks completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in GetMyTasks");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTasks(int? statusId)
        {
            _logger.LogInformation("GetAllTasks request received with statusId {StatusId}", statusId);
            //try
            //{
                var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(roleIdClaim) || int.Parse(roleIdClaim) != (int)eRole.Admin)
                {
                    var errorResponse = ResponseMessage.Unauthorized("Only administrators can view all tasks");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _taskService.GetAllTasksAsync(statusId);
                _logger.LogInformation("GetAllTasks completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in GetAllTasks");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTaskById(int taskId)
        {
            _logger.LogInformation("DeleteTask request received for TaskId {TaskId}", taskId);
            //try
            //{
                if (taskId <= 0)
                {
                    _logger.LogWarning("DeleteTask validation failed");
                    var errorResponse = ResponseMessage.BadRequest("Valid task ID is required");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                var response = await _taskService.DeleteTaskByIdAsync(taskId);
                _logger.LogInformation("DeleteTask completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in DeleteTask");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpPost("update-task")]
        public async Task<IActionResult> UpdateTask([FromQuery] int taskId, [FromBody] UpdateTaskRequestDTO dto)
        {
            _logger.LogInformation("UpdateTask request received for TaskId {TaskId}", taskId);
            //try
            //{
                if (taskId <= 0)
                {
                    var errorResponse = ResponseMessage.BadRequest("Valid task ID is required");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }
                var response = await _taskService.UpdateTaskByIdAsync(taskId, dto);
                _logger.LogInformation("UpdateTask completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in UpdateTask");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            _logger.LogInformation("GetDashboard request received");
            //try
            //{
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    var errorResponse = ResponseMessage.Unauthorized("Invalid token");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }

                int currentUserId = int.Parse(userIdClaim);
                bool isAdmin = !string.IsNullOrEmpty(roleIdClaim) && int.Parse(roleIdClaim) == (int)eRole.Admin;

                // If admin, pass null to get all users' data; otherwise pass userId for own data only
                int? userId = isAdmin ? (int?)null : currentUserId;
                var response = await _taskService.GetDashboardAsync(userId);
                _logger.LogInformation("GetDashboard completed with status code {StatusCode}",response.statusCode);
                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in GetDashboard");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }

        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            _logger.LogInformation("GetUserProfile request received");
            //try
            //{
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    var errorResponse = ResponseMessage.Unauthorized("Invalid token");
                    return StatusCode(errorResponse.statusCode, errorResponse);
                }
                int currentUserId = int.Parse(userIdClaim);
                var response = await _taskService.GetUserProfileAsync(currentUserId);
                _logger.LogInformation("GetUserProfile completed with status code {StatusCode}",response.statusCode);

                return StatusCode(response.statusCode, response);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unhandled exception in GetUserProfile");
            //    var errorResponse = ResponseMessage.Error("Internal server error: " + ex.Message);
            //    return StatusCode(errorResponse.statusCode, errorResponse);
            //}
        }
    }
}

using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManager.Api.Controllers;
using TaskManager.Business.Services.Task;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Enum;
using System.Threading.Tasks;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _mockService;
    private readonly Mock<ILogger<TaskController>> _mockLogger;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _mockService = new Mock<ITaskService>();
        _mockLogger = new Mock<ILogger<TaskController>>();
        _controller = new TaskController(_mockService.Object, _mockLogger.Object);
    }

    private void SetUserClaims(int userId, int roleId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, roleId.ToString())
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    private void SetNoClaimsContext()
    {
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ============================
    // CREATE TASK
    // ============================

    [Fact]
    public async Task CreateTask_ShouldReturnUnauthorized_WhenClaimsAreMissing()
    {
        SetNoClaimsContext();
        var request = new CreateTaskRequestDTO { TaskTitle = "Task", AssignedUserId = 2 };

        var result = await _controller.CreateTask(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        SetUserClaims(1, 2);

        var result = await _controller.CreateTask(null);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        SetUserClaims(1, 2);
        var request = new CreateTaskRequestDTO { TaskTitle = "   ", AssignedUserId = 2 };

        var result = await _controller.CreateTask(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task CreateTask_ShouldCallServiceAndReturnItsResponse_WhenRequestIsValid()
    {
        SetUserClaims(1, 2);
        var request = new CreateTaskRequestDTO { TaskTitle = "Valid Task", AssignedUserId = 3 };
        var serviceResponse = ResponseMessage.Ok("Task created");
        _mockService.Setup(s => s.CreateTaskAsync(request, 1, 2)).ReturnsAsync(serviceResponse);

        var result = await _controller.CreateTask(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.CreateTaskAsync(request, 1, 2), Times.Once);
    }

    // ============================
    // GET TASK BY ID
    // ============================

    [Fact]
    public async Task GetTaskById_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _controller.GetTaskById(0);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetTaskById_ShouldCallServiceAndReturnItsResponse_WhenTaskIdIsValid()
    {
        var serviceResponse = ResponseMessage.Ok("Task found");
        _mockService.Setup(s => s.GetTaskByIdAsync(5)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetTaskById(5);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetTaskByIdAsync(5), Times.Once);
    }

    // ============================
    // GET MY TASKS
    // ============================

    [Fact]
    public async Task GetMyTasks_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        SetNoClaimsContext();

        var result = await _controller.GetMyTasks(null);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetMyTasks_ShouldCallServiceWithUserIdAndStatus_WhenClaimsAreValid()
    {
        SetUserClaims(1, 2);
        var serviceResponse = ResponseMessage.Ok("My tasks");
        _mockService.Setup(s => s.GetMyTasksAsync(1, null)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetMyTasks(null);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetMyTasksAsync(1, null), Times.Once);
    }

    // ============================
    // GET ALL TASKS
    // ============================

    [Fact]
    public async Task GetAllTasks_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
    {
        SetUserClaims(1, 2); // roleId 2 is not admin

        var result = await _controller.GetAllTasks(null);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllTasks_ShouldCallServiceAndReturnItsResponse_WhenUserIsAdmin()
    {
        SetUserClaims(1, (int)eRole.Admin);
        var serviceResponse = ResponseMessage.Ok("All tasks");
        _mockService.Setup(s => s.GetAllTasksAsync(null)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetAllTasks(null);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetAllTasksAsync(null), Times.Once);
    }

    // ============================
    // DELETE TASK BY ID
    // ============================

    [Fact]
    public async Task DeleteTaskById_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _controller.DeleteTaskById(0);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task DeleteTaskById_ShouldCallServiceAndReturnItsResponse_WhenTaskIdIsValid()
    {
        var serviceResponse = ResponseMessage.Ok("Task deleted");
        _mockService.Setup(s => s.DeleteTaskByIdAsync(5)).ReturnsAsync(serviceResponse);

        var result = await _controller.DeleteTaskById(5);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.DeleteTaskByIdAsync(5), Times.Once);
    }

    // ============================
    // UPDATE TASK
    // ============================

    [Fact]
    public async Task UpdateTask_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _controller.UpdateTask(0, new UpdateTaskRequestDTO());

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_ShouldCallServiceAndReturnItsResponse_WhenTaskIdIsValid()
    {
        var dto = new UpdateTaskRequestDTO { TaskTitle = "Updated Title" };
        var serviceResponse = ResponseMessage.Ok("Task updated");
        _mockService.Setup(s => s.UpdateTaskByIdAsync(3, dto)).ReturnsAsync(serviceResponse);

        var result = await _controller.UpdateTask(3, dto);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.UpdateTaskByIdAsync(3, dto), Times.Once);
    }

    // ============================
    // GET DASHBOARD
    // ============================

    [Fact]
    public async Task GetDashboard_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        SetNoClaimsContext();

        var result = await _controller.GetDashboard();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetDashboard_ShouldPassNullToService_WhenUserIsAdmin()
    {
        // Admin gets all users' dashboard data, so controller passes null
        SetUserClaims(1, (int)eRole.Admin);
        var serviceResponse = ResponseMessage.Ok("Admin dashboard");
        _mockService.Setup(s => s.GetDashboardAsync(null)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDashboard();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetDashboardAsync(null), Times.Once);
    }

    [Fact]
    public async Task GetDashboard_ShouldPassUserIdToService_WhenUserIsNotAdmin()
    {
        // Non-admin only sees their own data, so controller passes their userId
        SetUserClaims(1, 2);
        var serviceResponse = ResponseMessage.Ok("User dashboard");
        _mockService.Setup(s => s.GetDashboardAsync(1)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetDashboard();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetDashboardAsync(1), Times.Once);
    }

    // ============================
    // GET USER PROFILE
    // ============================

    [Fact]
    public async Task GetUserProfile_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        SetNoClaimsContext();

        var result = await _controller.GetUserProfile();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetUserProfile_ShouldCallServiceWithUserId_WhenClaimIsValid()
    {
        SetUserClaims(7, 2);
        var serviceResponse = ResponseMessage.Ok("Profile data");
        _mockService.Setup(s => s.GetUserProfileAsync(7)).ReturnsAsync(serviceResponse);

        var result = await _controller.GetUserProfile();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetUserProfileAsync(7), Times.Once);
    }
}
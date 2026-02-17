using Xunit;
using Moq;
using TaskManager.Business.Services.Task;
using TaskManager.Data.Repositories.Task;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;
using TaskManager.Model.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockTaskRepository.Object);
    }

    // ============================
    // CREATE TASK
    // ============================

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        var dto = new CreateTaskRequestDTO { TaskTitle = "", AssignedUserId = 2 };

        var result = await _taskService.CreateTaskAsync(dto, 1, 2);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnBadRequest_WhenAssignedUserIdIsZero()
    {
        var dto = new CreateTaskRequestDTO { TaskTitle = "Test Task", AssignedUserId = 0 };

        var result = await _taskService.CreateTaskAsync(dto, 1, 2);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnBadRequest_WhenAdminAssignsTaskToThemselves()
    {
        // Admin userId = 1, AssignedUserId = 1 — same person, not allowed
        var dto = new CreateTaskRequestDTO { TaskTitle = "Test Task", AssignedUserId = 1 };

        var result = await _taskService.CreateTaskAsync(dto, 1, (int)eRole.Admin);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnUnauthorized_WhenNonAdminAssignsTaskToSomeoneElse()
    {
        // Non-admin userId = 1 trying to assign to userId = 2 — not allowed
        var dto = new CreateTaskRequestDTO { TaskTitle = "Test Task", AssignedUserId = 2 };

        var result = await _taskService.CreateTaskAsync(dto, 1, 2);

        Assert.Equal(401, result.statusCode);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnNotFound_WhenAssignedUserDoesNotExist()
    {
        var dto = new CreateTaskRequestDTO { TaskTitle = "Test Task", AssignedUserId = 5 };

        _mockTaskRepository.Setup(r => r.GetUserByIdAsync(5))
            .ReturnsAsync(ResponseMessage.NotFound("User not found"));

        var result = await _taskService.CreateTaskAsync(dto, 1, (int)eRole.Admin);

        Assert.Equal(404, result.statusCode);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnOk_WhenRequestIsValid()
    {
        var dto = new CreateTaskRequestDTO { TaskTitle = "Test Task", AssignedUserId = 5 };
        var user = new User { UserId = 5, Email = "user@test.com" };

        _mockTaskRepository.Setup(r => r.GetUserByIdAsync(5))
            .ReturnsAsync(ResponseMessage.Ok(user));

        _mockTaskRepository.Setup(r => r.CreateAsync(It.IsAny<TaskManager.Model.Model.DataLayer.Task>()))
            .ReturnsAsync(ResponseMessage.Ok("Task created"));

        var result = await _taskService.CreateTaskAsync(dto, 1, (int)eRole.Admin);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // GET TASK BY ID
    // ============================

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _taskService.GetTaskByIdAsync(0);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _mockTaskRepository.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync(ResponseMessage.NotFound("Task not found"));

        var result = await _taskService.GetTaskByIdAsync(99);

        Assert.Equal(404, result.statusCode);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnOk_WhenTaskExists()
    {
        var task = new TaskManager.Model.Model.DataLayer.Task { TaskId = 1, TaskTitle = "Test Task" };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ResponseMessage.Ok(task));

        var result = await _taskService.GetTaskByIdAsync(1);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // GET MY TASKS
    // ============================

    [Fact]
    public async Task GetMyTasksAsync_ShouldReturnBadRequest_WhenUserIdIsZeroOrLess()
    {
        var result = await _taskService.GetMyTasksAsync(0, null);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task GetMyTasksAsync_ShouldReturnOk_WhenUserHasTasks()
    {
        var tasks = new List<TaskManager.Model.Model.DataLayer.Task>
        {
            new TaskManager.Model.Model.DataLayer.Task { TaskId = 1, TaskTitle = "Task 1" }
        };

        _mockTaskRepository.Setup(r => r.GetTasksByUserIdAsync(1, null))
            .ReturnsAsync(ResponseMessage.Ok(tasks));

        var result = await _taskService.GetMyTasksAsync(1, null);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // GET ALL TASKS
    // ============================

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnNotFound_WhenNoTasksExist()
    {
        _mockTaskRepository.Setup(r => r.GetAllTasksAsync(null))
            .ReturnsAsync(ResponseMessage.Ok(null!));

        var result = await _taskService.GetAllTasksAsync(null);

        Assert.Equal(404, result.statusCode);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnOk_WhenTasksExist()
    {
        var tasks = new List<TaskManager.Model.Model.DataLayer.Task>
        {
            new TaskManager.Model.Model.DataLayer.Task { TaskId = 1, TaskTitle = "Task 1", UserId = null }
        };

        _mockTaskRepository.Setup(r => r.GetAllTasksAsync(null))
            .ReturnsAsync(ResponseMessage.Ok(tasks));

        var result = await _taskService.GetAllTasksAsync(null);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // DELETE TASK
    // ============================

    [Fact]
    public async Task DeleteTaskByIdAsync_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _taskService.DeleteTaskByIdAsync(0);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task DeleteTaskByIdAsync_ShouldReturnOk_WhenTaskIsDeletedSuccessfully()
    {
        _mockTaskRepository.Setup(r => r.DeleteTaskByIdAsync(1))
            .ReturnsAsync(ResponseMessage.Ok("Deleted"));

        var result = await _taskService.DeleteTaskByIdAsync(1);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // UPDATE TASK
    // ============================

    [Fact]
    public async Task UpdateTaskByIdAsync_ShouldReturnBadRequest_WhenTaskIdIsZeroOrLess()
    {
        var result = await _taskService.UpdateTaskByIdAsync(0, new UpdateTaskRequestDTO());

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task UpdateTaskByIdAsync_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _mockTaskRepository.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync(ResponseMessage.NotFound("Task not found"));

        var result = await _taskService.UpdateTaskByIdAsync(99, new UpdateTaskRequestDTO());

        Assert.Equal(404, result.statusCode);
    }

    [Fact]
    public async Task UpdateTaskByIdAsync_ShouldReturnOk_WhenTaskIsUpdatedSuccessfully()
    {
        var task = new TaskManager.Model.Model.DataLayer.Task { TaskId = 1, TaskTitle = "Old Title" };
        var dto = new UpdateTaskRequestDTO { TaskTitle = "New Title" };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ResponseMessage.Ok(task));

        _mockTaskRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskManager.Model.Model.DataLayer.Task>()))
            .ReturnsAsync(ResponseMessage.Ok("Updated"));

        var result = await _taskService.UpdateTaskByIdAsync(1, dto);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // GET DASHBOARD
    // ============================

    [Fact]
    public async Task GetDashboardAsync_ShouldReturnBadRequest_WhenUserIdIsZeroOrLess()
    {
        var result = await _taskService.GetDashboardAsync(0);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task GetDashboardAsync_ShouldReturnOk_WhenCalledByNonAdminWithValidUserId()
    {
        var tasks = new List<TaskManager.Model.Model.DataLayer.Task>
        {
            new TaskManager.Model.Model.DataLayer.Task { TaskId = 1, TaskTitle = "Task 1", UserId = null }
        };

        _mockTaskRepository.Setup(r => r.GetDashboardDataAsync(1))
            .ReturnsAsync(ResponseMessage.Ok(tasks));

        _mockTaskRepository.Setup(r => r.GetCompletedTasksCountByMonthAsync(1, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(ResponseMessage.Ok(0));

        var result = await _taskService.GetDashboardAsync(1);

        Assert.Equal(200, result.statusCode);
    }

    // ============================
    // GET USER PROFILE
    // ============================

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnBadRequest_WhenUserIdIsZeroOrLess()
    {
        var result = await _taskService.GetUserProfileAsync(0);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _mockTaskRepository.Setup(r => r.GetUserByIdAsync(99))
            .ReturnsAsync(ResponseMessage.NotFound("User not found"));

        var result = await _taskService.GetUserProfileAsync(99);

        Assert.Equal(404, result.statusCode);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnOk_WhenUserExists()
    {
        var user = new User { UserId = 1, FullName = "Test User", Email = "test@test.com", RoleId = 2 };
        var tasks = new List<TaskManager.Model.Model.DataLayer.Task>();

        _mockTaskRepository.Setup(r => r.GetUserByIdAsync(1))
            .ReturnsAsync(ResponseMessage.Ok(user));

        _mockTaskRepository.Setup(r => r.GetTasksByUserIdAsync(1, null))
            .ReturnsAsync(ResponseMessage.Ok(tasks));

        var result = await _taskService.GetUserProfileAsync(1);

        Assert.Equal(200, result.statusCode);
    }
}
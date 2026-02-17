using Xunit;
using Moq;
using TaskManager.Business.Services.Configuration;
using TaskManager.Data.Repositories.Configuration;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;
using TaskStatus = TaskManager.Model.Model.DataLayer.TaskStatus;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

public class ConfigurationServiceTests
{
    private readonly Mock<IConfigurationRepository> _mockConfigurationRepository;
    private readonly ConfigurationService _configurationService;

    public ConfigurationServiceTests()
    {
        _mockConfigurationRepository = new Mock<IConfigurationRepository>();
        _configurationService = new ConfigurationService(_mockConfigurationRepository.Object);
    }

    // ============================
    // GET ALL PRIORITIES
    // ============================

    [Fact]
    public async Task GetAllPrioritiesAsync_ShouldReturnOk_WhenRepositoryReturnsData()
    {
        var priorities = new List<TaskPriority>
        {
            new TaskPriority { TaskPriorityId = 1, TaskPriorityName = "High" },
            new TaskPriority { TaskPriorityId = 2, TaskPriorityName = "Low" }
        };

        _mockConfigurationRepository.Setup(r => r.GetPrioritiesAsync())
            .ReturnsAsync(ResponseMessage.Ok("Priorities"));

        var result = await _configurationService.GetAllPrioritiesAsync();

        Assert.Equal(200, result.statusCode);
    }

    [Fact]
    public async Task GetAllPrioritiesAsync_ShouldReturn500_WhenRepositoryThrowsAnException()
    {
        _mockConfigurationRepository.Setup(r => r.GetPrioritiesAsync())
            .ThrowsAsync(new Exception("DB connection failed"));

        var result = await _configurationService.GetAllPrioritiesAsync();

        Assert.Equal(500, result.statusCode);
    }

    // ============================
    // GET ALL STATUSES
    // ============================

    [Fact]
    public async Task GetAllStatusesAsync_ShouldReturnOk_WhenRepositoryReturnsData()
    {
        var statuses = new List<TaskStatus>
        {
            new TaskStatus { TaskStatusId = 1, TaskStatusName = "Pending" },
            new TaskStatus { TaskStatusId = 2, TaskStatusName = "Completed" }
        };

        _mockConfigurationRepository.Setup(r => r.GetStatusesAsync())
            .ReturnsAsync(ResponseMessage.Ok("Statuses"));

        var result = await _configurationService.GetAllStatusesAsync();

        Assert.Equal(200, result.statusCode);
    }

    [Fact]
    public async Task GetAllStatusesAsync_ShouldReturn500_WhenRepositoryThrowsAnException()
    {
        _mockConfigurationRepository.Setup(r => r.GetStatusesAsync())
            .ThrowsAsync(new Exception("DB connection failed"));

        var result = await _configurationService.GetAllStatusesAsync();

        Assert.Equal(500, result.statusCode);
    }
}
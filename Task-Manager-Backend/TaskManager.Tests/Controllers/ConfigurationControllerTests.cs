using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Api.Controllers;
using TaskManager.Business.Services.Configuration;
using TaskManager.Model.Model.Common;
using System.Threading.Tasks;

public class ConfigurationControllerTests
{
    private readonly Mock<IConfigurationService> _mockService;
    private readonly Mock<ILogger<ConfigurationController>> _mockLogger;
    private readonly ConfigurationController _controller;

    public ConfigurationControllerTests()
    {
        _mockService = new Mock<IConfigurationService>();
        _mockLogger = new Mock<ILogger<ConfigurationController>>();
        _controller = new ConfigurationController(_mockService.Object, _mockLogger.Object);
    }

    // ============================
    // GET ALL PRIORITIES
    // ============================

    [Fact]
    public async Task GetAllPriority_ShouldCallServiceAndReturnItsResponse_WhenRequestIsReceived()
    {
        var serviceResponse = ResponseMessage.Ok("Priorities fetched");
        _mockService.Setup(s => s.GetAllPrioritiesAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetAllPriority();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetAllPrioritiesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPriority_ShouldReturn500_WhenServiceThrowsAnException()
    {
        _mockService.Setup(s => s.GetAllPrioritiesAsync()).ThrowsAsync(new Exception("DB error"));

        var result = await _controller.GetAllPriority();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    // ============================
    // GET ALL STATUSES
    // ============================

    [Fact]
    public async Task GetAllStatuses_ShouldCallServiceAndReturnItsResponse_WhenRequestIsReceived()
    {
        var serviceResponse = ResponseMessage.Ok("Statuses fetched");
        _mockService.Setup(s => s.GetAllStatusesAsync()).ReturnsAsync(serviceResponse);

        var result = await _controller.GetAllStatuses();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, objectResult.StatusCode);
        _mockService.Verify(s => s.GetAllStatusesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllStatuses_ShouldReturn500_WhenServiceThrowsAnException()
    {
        _mockService.Setup(s => s.GetAllStatusesAsync()).ThrowsAsync(new Exception("DB error"));

        var result = await _controller.GetAllStatuses();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Controllers;
using TaskManager.Business.Services.Auth;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;
using System.Threading.Tasks;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task Login_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        LoginRequestDTO request = null;

        // Act
        var result = await _controller.Login(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, statusResult.StatusCode);
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsResponseFromService()
    {
        // Arrange
        var request = new LoginRequestDTO { Username = "test", Password = "pass" };
        var serviceResponse = ResponseMessage.Ok("Login successful");

        _mockAuthService
            .Setup(s => s.Login(request))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, statusResult.StatusCode);
        Assert.Equal(serviceResponse.message, ((ResponseMessage)statusResult.Value).message);
    }

    [Fact]
    public async Task Signup_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new SignupRequestDTO { Email = "", Password = "", FullName = "" };

        // Act
        var result = await _controller.Signup(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, statusResult.StatusCode);
    }

    [Fact]
    public async Task Signup_ValidRequest_ReturnsResponseFromService()
    {
        // Arrange
        var request = new SignupRequestDTO { Email = "test@mail.com", Password = "pass", FullName = "Test User" };
        var serviceResponse = ResponseMessage.Ok("Signup successful");

        _mockAuthService
            .Setup(s => s.Signup(request))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.Signup(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(serviceResponse.statusCode, statusResult.StatusCode);
        Assert.Equal(serviceResponse.message, ((ResponseMessage)statusResult.Value).message);
    }
}

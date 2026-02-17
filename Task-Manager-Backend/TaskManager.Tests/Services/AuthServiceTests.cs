using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using TaskManager.Business.Services.Auth;
using TaskManager.Business.Helpers;
using TaskManager.Data.Repositories.Auth;
using TaskManager.Model.Model.BusinessLayer.RequestDTO;
using TaskManager.Model.Model.Common;
using TaskManager.Model.Model.DataLayer;
using Task = System.Threading.Tasks.Task;
public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _mockAuthRepository;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockAuthRepository = new Mock<IAuthRepository>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["JwtSettings:ExpirationMinutes"]).Returns("60");

        _authService = new AuthService(
            _mockAuthRepository.Object,
            _mockJwtTokenGenerator.Object,
            _mockConfiguration.Object
        );
    }

    // ============================
    // LOGIN
    // ============================

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenRepositoryReturnsFailure()
    {
        var request = new LoginRequestDTO { Username = "test@test.com", Password = "pass123" };

        _mockAuthRepository.Setup(r => r.Login(request))
            .ReturnsAsync(ResponseMessage.Unauthorized("Invalid username or password"));

        var result = await _authService.Login(request);

        Assert.Equal(401, result.statusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        var request = new LoginRequestDTO { Username = "test@test.com", Password = "wrongpassword" };

        // Repository returns success with a user whose password is a BCrypt hash of a different password
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            RoleId = 2
        };

        _mockAuthRepository.Setup(r => r.Login(request))
            .ReturnsAsync(ResponseMessage.Ok(user));

        var result = await _authService.Login(request);

        Assert.Equal(401, result.statusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreCorrect()
    {
        var plainPassword = "correctpassword";
        var request = new LoginRequestDTO { Username = "test@test.com", Password = plainPassword };

        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FullName = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword(plainPassword),
            RoleId = 2
        };

        _mockAuthRepository.Setup(r => r.Login(request))
            .ReturnsAsync(ResponseMessage.Ok(user));

        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(user))
            .Returns("fake-jwt-token");

        var result = await _authService.Login(request);

        Assert.Equal(200, result.statusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn500_WhenRepositoryThrowsAnException()
    {
        var request = new LoginRequestDTO { Username = "test@test.com", Password = "pass123" };

        _mockAuthRepository.Setup(r => r.Login(request))
            .ThrowsAsync(new Exception("DB connection failed"));

        var result = await _authService.Login(request);

        Assert.Equal(500, result.statusCode);
    }

    // ============================
    // SIGNUP
    // ============================

    [Fact]
    public async Task Signup_ShouldReturnBadRequest_WhenEmailIsMissing()
    {
        var request = new SignupRequestDTO { Email = "", Password = "pass123", FullName = "Test User" };

        var result = await _authService.Signup(request);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task Signup_ShouldReturnBadRequest_WhenPasswordIsMissing()
    {
        var request = new SignupRequestDTO { Email = "test@test.com", Password = "", FullName = "Test User" };

        var result = await _authService.Signup(request);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task Signup_ShouldReturnBadRequest_WhenFullNameIsMissing()
    {
        var request = new SignupRequestDTO { Email = "test@test.com", Password = "pass123", FullName = "" };

        var result = await _authService.Signup(request);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task Signup_ShouldReturnBadRequest_WhenRepositoryReturnsFailure()
    {
        var request = new SignupRequestDTO { Email = "existing@test.com", Password = "pass123", FullName = "Test User" };

        _mockAuthRepository.Setup(r => r.Signup(It.IsAny<SignupRequestDTO>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(ResponseMessage.BadRequest("Email already exists"));

        var result = await _authService.Signup(request);

        Assert.Equal(400, result.statusCode);
    }

    [Fact]
    public async Task Signup_ShouldReturnCreated_WhenSignupIsSuccessful()
    {
        var request = new SignupRequestDTO { Email = "newuser@test.com", Password = "pass123", FullName = "New User" };

        _mockAuthRepository.Setup(r => r.Signup(It.IsAny<SignupRequestDTO>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(ResponseMessage.Created());

        var result = await _authService.Signup(request);

        Assert.Equal(201, result.statusCode);
    }

    [Fact]
    public async Task Signup_ShouldReturn500_WhenRepositoryThrowsAnException()
    {
        var request = new SignupRequestDTO { Email = "test@test.com", Password = "pass123", FullName = "Test User" };

        _mockAuthRepository.Setup(r => r.Signup(It.IsAny<SignupRequestDTO>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("DB connection failed"));

        var result = await _authService.Signup(request);

        Assert.Equal(500, result.statusCode);
    }
}
using Moq;
using Xunit;
using TipJar.Application.Services;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;
using TipJar.Domain.Interfaces.Security;
using TipJar.Application.Dtos.UserDto;
using TipJar.Application.Exceptions;

namespace TipJar.Tests.Services.AuthServices;

public class LoginAsyncTests
{
    private readonly Mock<IUserRepository> _userRespositoryMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly AuthService _authService;

    public LoginAsyncTests()
    {
        _authService = new AuthService(
            _userRespositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task ValidUser_ReturnsToken()
    {
        // Arrange
        string username = "testuser";
        string password = "testpassword";
        string passwordHash = "hashedpassword";
        Guid userId = Guid.NewGuid();
        string expectedToken = "valid.jwt.token";

        _userRespositoryMock.Setup(repo => repo.GetForLoginAsync(username))
            .ReturnsAsync(new UserLoginInfoDto
            {
                Id = userId,
                Username = username,
                PasswordHash = passwordHash
            });

        _passwordHasherMock.Setup(hasher => hasher.Verify(password, passwordHash))
            .Returns(true);

        _jwtServiceMock.Setup(jwt => jwt.GenerateToken(userId))
            .Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);

        // Verify that user repository was called once
        _userRespositoryMock.Verify(repo => repo.GetForLoginAsync(It.IsAny<string>()), Times.Once);

        // Verify that the password hasher was called once
        _passwordHasherMock.Verify(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        // Verify that the JWT service was called once 
        _jwtServiceMock.Verify(jwt => jwt.GenerateToken(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task InvalidUsername_ThrowUnauthorizedException()
    {
        // Arrange
        _userRespositoryMock.Setup(repo => repo.GetForLoginAsync(It.IsAny<string>()))
            .ReturnsAsync((UserLoginInfoDto?)null);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync("invalidUsername", "anyPassword"));

        // Assert
        Assert.Equal("Invalid username or password.", exception.Message);

        // Verify that user repository was called once
        _userRespositoryMock.Verify(repo => repo.GetForLoginAsync(It.IsAny<string>()), Times.Once);

        // Verify that the JWT service was never called
        _jwtServiceMock.Verify(jwt => jwt.GenerateToken(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task InvalidPassword_ThrowUnauthorizedException()
    {
        // Arrange
        string username = "testuser";
        string password = "testpassword";
        string passwordHash = "hashedpassword";
        Guid userId = Guid.NewGuid();

        _userRespositoryMock.Setup(repo => repo.GetForLoginAsync(username))
            .ReturnsAsync(new UserLoginInfoDto
            {
                Id = userId,
                Username = username,
                PasswordHash = passwordHash
            });
        
        _passwordHasherMock.Setup(hasher => hasher.Verify(password, passwordHash))
            .Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(username, password));

        // Assert
        Assert.Equal("Invalid username or password.", exception.Message);

        // Verify that user repository was called once
        _userRespositoryMock.Verify(repo => repo.GetForLoginAsync(username), Times.Once);

        // Verify that the JWT service was never called
        _jwtServiceMock.Verify(jwt => jwt.GenerateToken(It.IsAny<Guid>()), Times.Never);
    }
}
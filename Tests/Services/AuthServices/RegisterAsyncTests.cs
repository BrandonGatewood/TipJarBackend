using Moq;
using TipJar.Application.Exceptions;
using TipJar.Application.Services;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Security;
using TipJar.Domain.Interfaces.Services;
using Xunit;

namespace TipJar.Tests.Services.AuthServices;

public class RegisterAsyncTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly AuthService _authService;

    public RegisterAsyncTests()
    {
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task ValidRegistration_ReturnsToken()
    {
        // Arrange
        string username = "user";
        string password = "password";
        string hashedPassword = "hashed";
        string expectedToken = "valid.jwt.token";

        _userRepositoryMock.Setup(repo => repo.ExistsByUsernameAsync(username))
            .ReturnsAsync(false);
        _passwordHasherMock.Setup(hasher => hasher.Hash(password))
            .Returns(hashedPassword);
        _jwtServiceMock.Setup(jwt => jwt.GenerateToken(It.IsAny<Guid>()))
            .Returns(expectedToken);

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);

        // Verify that user repository was called once
        _userRepositoryMock.Verify(repo => repo.ExistsByUsernameAsync(username), Times.Once);

        // Verify that the password hasher was called once
        _passwordHasherMock.Verify(hasher => hasher.Hash(password), Times.Once);

        // Verify that the JWT service was called once 
        _jwtServiceMock.Verify(jwt => jwt.GenerateToken(It.IsAny<Guid>()), Times.Once);

        // Verify that the user repository's AddAsync and SaveChangesAsync methods were called once
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Username == username && u.PasswordHash == hashedPassword)), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task InvalidUsername_ThrowsConflictException()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync("user", "password"));

        // Assert
        Assert.Equal("username already exists.", exception.Message);

        // Verify that user repository was called once
        _userRepositoryMock.Verify(repo => repo.ExistsByUsernameAsync(It.IsAny<string>()), Times.Once);

        // Verify that the JWT service was never called
        _jwtServiceMock.Verify(jwt => jwt.GenerateToken(It.IsAny<Guid>()), Times.Never);

        // Verify that user was never saved
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);

        // Verify that password hasher was never called
        _passwordHasherMock.Verify(h => h.Hash(It.IsAny<string>()), Times.Never);
    }
}
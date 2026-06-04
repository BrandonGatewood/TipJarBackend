using Moq;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipJar.Application.Services;

namespace TipJar.Tests.Services.JwtServices;

public class GenerateTokenTests
{
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly JwtService _jwtService;

    public GenerateTokenTests()
    {
        _jwtService = new JwtService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_ReturnsValidToken()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _configurationMock.Setup(c => c["JWT:KEY"])
            .Returns("super_secret_key_1234567890_super_secure");   
        _configurationMock.Setup(c => c["JWT:ISSUER"])
            .Returns("test_issuer");
        _configurationMock.Setup(c => c["JWT:AUDIENCE"])
            .Returns("test_audience");
        _configurationMock.Setup(c => c["JWT:EXPIRESINMINUTES"])
            .Returns("60");
        
        // Act
        var result = _jwtService.GenerateToken(userId);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        Assert.Equal(userId.ToString(), claim.Value);

        Assert.Equal("test_issuer", jwt.Issuer);
        Assert.Equal("test_audience", jwt.Audiences.First());
    }

    [Fact]
    public void MissingKey_ThrowsInvalidOperationException()
    {
        // Arrange
        _configurationMock.Setup(c => c["JWT:KEY"])
            .Returns((string?)null);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => _jwtService.GenerateToken(Guid.NewGuid()));

        // Assert
        Assert.Equal("JWT:KEY is not configured.", exception.Message);
    }

    [Fact]
    public void InvalidExpiresInMinutes_UsesDefault()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _configurationMock.Setup(c => c["JWT:KEY"])
            .Returns("super_secret_key_1234567890_super_secure");   
        _configurationMock.Setup(c => c["JWT:ISSUER"])
            .Returns("test_issuer");
        _configurationMock.Setup(c => c["JWT:AUDIENCE"])
            .Returns("test_audience");
        _configurationMock.Setup(c => c["JWT:EXPIRESINMINUTES"])
            .Returns("invalid_number");
        DateTime before = DateTime.UtcNow;

        // Act
        var result = _jwtService.GenerateToken(userId);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result);

        // Assert
        Assert.True(jwt.ValidTo > before.AddMinutes(59));
        Assert.True(jwt.ValidTo < before.AddMinutes(61));
    } 
}
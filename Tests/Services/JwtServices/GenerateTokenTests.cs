using Moq;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using TipJar.Application.Services;
using TipJar.Infrastructure.Security.Jwt;

namespace TipJar.Tests.Services.JwtServices;

public class GenerateTokenTests
{
    [Fact]
    public void GenerateToken_ReturnsValidToken()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        var options = Options.Create(new JwtOptions
        {
            Key = "super_secret_key_1234567890_super_secure",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpiresInMinutes = 60
        });

        var jwtService = new JwtService(options); 
        
        // Act
        var result = jwtService.GenerateToken(userId);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result);
        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        Assert.Equal(userId.ToString(), claim.Value);
        Assert.Equal("test_issuer", jwt.Issuer);
        Assert.Single(jwt.Audiences);
        Assert.Equal("test_audience", jwt.Audiences.First());
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}
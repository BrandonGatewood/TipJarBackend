using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using TipJar.Domain.Interfaces.Security;
using TipJar.Infrastructure.Security.Jwt;

namespace TipJar.Application.Services;

/// <summary>
/// Service responsible for generating JWT for authenticated users.
/// </summary>
public class JwtService(IOptions<JwtOptions> options) : IJwtService
{
    private readonly JwtOptions _options = options.Value;

    /// <summary>
    /// Generates a signed JWT for the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A JWT string representing the authenticated user..</returns>
    public string GenerateToken(Guid id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        // Create the signing key from options 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        // Define signing algorithm and credentials
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Build the JWT
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes),
            signingCredentials: creds
        );

        // Serialize token to string format
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

public class JwtService(IConfiguration config) : IJwtService
{
    private readonly IConfiguration _config = config;

    public string GenerateToken(Guid id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        var keyString = _config["JWT:KEY"] ?? throw new InvalidOperationException("JWT:KEY is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresIn = int.TryParse(_config["JWT:EXPIRESINMINUTES"], out var mins) ? mins : 60;

        var token = new JwtSecurityToken(
            issuer: _config["JWT:ISSUER"],
            audience: _config["JWT:AUDIENCE"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
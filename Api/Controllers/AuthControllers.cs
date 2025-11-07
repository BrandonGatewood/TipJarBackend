using Microsoft.AspNetCore.Mvc;
using TipJar.Application.Dtos.AuthDto;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        return Ok(await _authService.LoginAsync(dto.Username, dto.Password));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        return Ok(await _authService.RegisterAsync(dto.Username, dto.Password));
    }
}
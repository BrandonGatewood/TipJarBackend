using Microsoft.AspNetCore.Mvc;
using TipJar.Application.Dtos.UserDto;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        return Ok(await _userService.GetUserInfoAsync());
    }

    [HttpPatch("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await _userService.ChangePasswordAsync(dto.CurrentPassword, dto.NewPassword);

        return Ok();
    }

    [HttpPatch("changeUsername")]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameDto dto)
    {
        await _userService.ChangeUsernameAsync(dto.NewUsername);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveUser()
    {
        await _userService.RemoveUserAsync();

        return Ok();
    }
}
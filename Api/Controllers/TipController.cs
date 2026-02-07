using Microsoft.AspNetCore.Mvc;
using TipJar.Application.Dtos.TipDto;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Api.Controllers;

[ApiController]
[Route("api/tips")]
public class TipController(ITipService tipService) : ControllerBase
{
    private readonly ITipService _tipService = tipService;

    [HttpPost]
    public async Task<IActionResult> AddTip([FromBody] AmountDto dto)
    {
        await _tipService.AddTipAsync(dto.Amount);
        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditTip(Guid id, [FromBody] EditTipDto dto)
    {
        await _tipService.EditTipAsync(id, dto.Amount, dto.CreatedAt);
        return Ok();
    }
}
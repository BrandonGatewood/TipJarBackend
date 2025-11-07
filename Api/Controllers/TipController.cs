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
    public async Task<IActionResult> AddTip([FromBody] AddTipDto dto)
    {
        return Ok(await _tipService.AddTipAsync(dto.Amount));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditTip(Guid id, [FromBody] EditTipDto dto)
    {
        return Ok(await _tipService.EditTipAsync(id, dto.Amount, dto.CreatedAt));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTip(Guid id)
    {
        await _tipService.DeleteTipAsync(id);
        
        return Ok();
    }

    [HttpGet("thisMonthsEarnings")]
    public async Task<IActionResult> GetThisMonthsEarnings()
    {
        return Ok(await _tipService.GetThisMonthsEarningsAsync());
    }

    [HttpGet("thisMonthsTips")]
    public async Task<IActionResult> GetThisMonthsTips()
    {
        return Ok(await _tipService.GetThisMonthsTipsAsync());
    }

    [HttpGet("quarterlyInfo")]
    public async Task<IActionResult> GetQuarterlyInfo([FromBody] SelectedQuarterDto dto)
    {
        return Ok(await _tipService.GetQuarterlyInfoAsync(dto.Year, dto.Quarter));
    }
}
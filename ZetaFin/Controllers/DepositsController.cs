using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositsController : ControllerBase
{
    private readonly IDepositService _depositService;

    public DepositsController(IDepositService depositService)
    {
        _depositService = depositService;
    }

    [HttpPost]
    public async Task<IActionResult> AddDeposit([FromBody] CreateDepositDto dto)
    {
        var deposit = await _depositService.AddDepositAsync(dto);
        return Ok(deposit);
    }

    [HttpGet("goal/{goalId}")]
    public async Task<IActionResult> GetDeposits(Guid goalId)
    {
        var deposits = await _depositService.GetDepositsByGoalIdAsync(goalId);
        return Ok(deposits);
    }
}

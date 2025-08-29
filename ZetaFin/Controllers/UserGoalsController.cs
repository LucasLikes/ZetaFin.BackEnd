using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserGoalsController : ControllerBase
{
    private readonly IUserGoalService _userGoalService;

    public UserGoalsController(IUserGoalService userGoalService)
    {
        _userGoalService = userGoalService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserGoalDto dto)
    {
        await _userGoalService.CreateUserGoalAsync(dto);
        return Ok();
    }

    [HttpPut("{goalId}/{userId}")]
    public async Task<IActionResult> UpdateMonthlyTarget(Guid goalId, Guid userId, [FromBody] decimal? customMonthlyTarget)
    {
        await _userGoalService.UpdateMonthlyTargetAsync(userId, goalId, customMonthlyTarget);
        return NoContent();
    }

    [HttpGet("{goalId}")]
    public async Task<IActionResult> GetByGoal(Guid goalId)
    {
        var result = await _userGoalService.GetUserGoalsByGoalIdAsync(goalId);
        return Ok(result);
    }
}

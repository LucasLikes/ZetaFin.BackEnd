using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalDto dto)
    {
        var createdGoal = await _goalService.CreateGoalAsync(dto);
        return CreatedAtAction(nameof(GetGoalById), new { id = createdGoal.Id }, createdGoal);
    }

    [HttpGet]
    public async Task<IActionResult> GetGoals()
    {
        var goals = await _goalService.GetGoalsAsync();
        return Ok(goals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGoalById(Guid id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null) return NotFound();
        return Ok(goal);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[AllowAnonymous]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var result = await _expenseService.GetExpensesByUserAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        return expense == null ? NotFound() : Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        var expense = await _expenseService.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var expense = await _expenseService.UpdateAsync(id, request);
        return Ok(expense);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _expenseService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("summary/{userId}")]
    public async Task<IActionResult> GetSummary(string userId)
    {
        var summary = await _expenseService.GetSummaryByCategoryAsync(userId);
        return Ok(summary);
    }
}

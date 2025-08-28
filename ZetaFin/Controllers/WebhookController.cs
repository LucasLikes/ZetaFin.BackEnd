using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.Interfaces;
using ZetaFin.Application.DTOs;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IGoalService _goalService;

    public WebhookController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost("mercadopago")]
    public async Task<IActionResult> HandlePixNotification([FromBody] MercadoPagoPixNotification notification)
    {
        // Exemplo: busca a meta
        var goal = await _goalService.GetGoalByIdAsync(notification.GoalId);
        if (goal == null) return NotFound("Goal not found.");

        // Atualiza o valor da meta com o aporte recebido
        await _goalService.AddDepositToGoalAsync(notification.GoalId, notification.Amount);

        return Ok("Deposit registered successfully.");
    }
}

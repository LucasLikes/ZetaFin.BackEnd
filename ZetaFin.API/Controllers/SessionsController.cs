using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using System.Security.Claims;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(
        IAuthenticationService authenticationService,
        ILogger<SessionsController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Listar todas as sessões ativas do usuário
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserSessionDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetActiveSessions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var sessions = await _authenticationService.GetActiveSessionsAsync(userId);

        return Ok(sessions);
    }

    /// <summary>
    /// Encerrar uma sessão específica
    /// </summary>
    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateSession(Guid sessionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authenticationService.TerminateSessionAsync(userId, sessionId);

        if (!result)
        {
            _logger.LogWarning("Failed to terminate session {SessionId} for user {UserId}", sessionId, userId);
            return NotFound(new { error = "Session not found" });
        }

        _logger.LogInformation("Session terminated: {SessionId} for user {UserId}", sessionId, userId);
        return Ok(new { message = "Session terminated successfully" });
    }

    /// <summary>
    /// Encerrar todas as sessões do usuário (com a exceção da atual, opcional)
    /// </summary>
    [HttpDelete("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TerminateAllSessions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authenticationService.TerminateAllSessionsAsync(userId);

        if (!result)
        {
            _logger.LogWarning("Failed to terminate all sessions for user {UserId}", userId);
            return BadRequest(new { error = "Failed to terminate all sessions" });
        }

        _logger.LogInformation("All sessions terminated for user: {UserId}", userId);
        return Ok(new { message = "All sessions terminated successfully" });
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WhatsAppAuthController : ControllerBase
{
    private readonly IWhatsAppAuthService _whatsAppAuthService;

    public WhatsAppAuthController(IWhatsAppAuthService whatsAppAuthService)
    {
        _whatsAppAuthService = whatsAppAuthService;
    }

    /// <summary>
    /// Autentica usuário via número WhatsApp e retorna JWT
    /// </summary>
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] WhatsAppAuthRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.WhatsAppNumber))
            return BadRequest(new { error = "WhatsApp number is required" });

        var result = await _whatsAppAuthService.AuthenticateByWhatsAppAsync(request.WhatsAppNumber);

        if (result == null)
            return NotFound(new { error = "WhatsApp number not linked to any user" });

        return Ok(result);
    }

    /// <summary>
    /// Vincula WhatsApp ao usuário logado
    /// </summary>
    [HttpPost("link")]
    [Authorize]
    public async Task<IActionResult> LinkWhatsApp([FromBody] LinkWhatsAppDto dto)
    {
        var success = await _whatsAppAuthService.LinkWhatsAppToUserAsync(dto.UserId, dto.WhatsAppNumber);

        if (!success)
            return BadRequest(new { error = "WhatsApp number already linked to another user" });

        return Ok(new { message = "WhatsApp linked successfully" });
    }

    /// <summary>
    /// Remove vinculação WhatsApp
    /// </summary>
    [HttpDelete("unlink/{userId}")]
    [Authorize]
    public async Task<IActionResult> UnlinkWhatsApp(Guid userId)
    {
        var success = await _whatsAppAuthService.UnlinkWhatsAppAsync(userId);

        if (!success)
            return NotFound(new { error = "No WhatsApp linked to this user" });

        return Ok(new { message = "WhatsApp unlinked successfully" });
    }

    /// <summary>
    /// Verifica se WhatsApp está vinculado
    /// </summary>
    [HttpGet("check/{whatsAppNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckLinked(string whatsAppNumber)
    {
        var isLinked = await _whatsAppAuthService.IsWhatsAppLinkedAsync(whatsAppNumber);
        return Ok(new { isLinked });
    }
}
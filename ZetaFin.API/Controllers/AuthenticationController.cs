using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using System.Security.Claims;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registrar novo usuário
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var (success, error, data) = await _authenticationService.RegisterAsync(dto, ipAddress, userAgent);

        if (!success)
        {
            _logger.LogWarning("Registration failed: {Error}", error);
            return BadRequest(new { error });
        }

        _logger.LogInformation("User registered successfully: {UserId}", data?.UserId);
        return Ok(data);
    }

    /// <summary>
    /// Login com email e senha
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        var deviceName = HttpContext.Request.Headers["X-Device-Name"].ToString() ?? "Web";
        var deviceType = HttpContext.Request.Headers["X-Device-Type"].ToString() ?? "Web";

        var (success, error, data) = await _authenticationService.LoginAsync(
            dto, deviceName, deviceType, ipAddress, userAgent);

        if (!success)
        {
            _logger.LogWarning("Login failed: {Error}", error);
            return Unauthorized(new { error });
        }

        _logger.LogInformation("User logged in: {UserId}", data?.UserId);
        return Ok(data);
    }

    /// <summary>
    /// Renovar token JWT usando refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var (success, error, data) = await _authenticationService.RefreshTokenAsync(dto, ipAddress, userAgent);

        if (!success)
        {
            _logger.LogWarning("Token refresh failed: {Error}", error);
            return Unauthorized(new { error });
        }

        return Ok(data);
    }

    /// <summary>
    /// Fazer logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var sessionIdClaim = User.FindFirst("session_id")?.Value;
        Guid? sessionId = sessionIdClaim != null && Guid.TryParse(sessionIdClaim, out var sid) ? sid : null;

        var result = await _authenticationService.LogoutAsync(userId, sessionId);

        if (!result)
        {
            return BadRequest(new { error = "Logout failed" });
        }

        _logger.LogInformation("User logged out: {UserId}", userId);
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Fazer logout de todas as sessões
    /// </summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authenticationService.LogoutAllAsync(userId);

        if (!result)
        {
            return BadRequest(new { error = "Logout all failed" });
        }

        _logger.LogInformation("User logged out from all sessions: {UserId}", userId);
        return Ok(new { message = "Logged out from all sessions successfully" });
    }

    /// <summary>
    /// Mudar senha
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var (success, error) = await _authenticationService.ChangePasswordAsync(userId, dto);

        if (!success)
        {
            _logger.LogWarning("Change password failed for user {UserId}: {Error}", userId, error);
            return BadRequest(new { error });
        }

        _logger.LogInformation("Password changed for user: {UserId}", userId);
        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Solicitar reset de senha (Forgot Password)
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        // Construir URL de reset (em produção, seria enviada via email)
        var resetUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/reset-password?token={{token}}";

        var (success, error) = await _authenticationService.ForgotPasswordAsync(dto.Email, resetUrl);

        if (!success)
        {
            _logger.LogWarning("Forgot password failed: {Error}", error);
            return BadRequest(new { error });
        }

        // Não revelar se email existe (LGPD/segurança)
        return Ok(new { message = "If the email exists, you will receive a password reset link" });
    }

    /// <summary>
    /// Reset de senha com token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var (success, error) = await _authenticationService.ResetPasswordAsync(dto);

        if (!success)
        {
            _logger.LogWarning("Reset password failed: {Error}", error);
            return BadRequest(new { error });
        }

        _logger.LogInformation("Password reset successfully for: {Email}", dto.Email);
        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Confirmar email
    /// </summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        var (success, error) = await _authenticationService.ConfirmEmailAsync(email, token);

        if (!success)
        {
            _logger.LogWarning("Email confirmation failed: {Error}", error);
            return BadRequest(new { error });
        }

        _logger.LogInformation("Email confirmed: {Email}", email);
        return Ok(new { message = "Email confirmed successfully" });
    }
}

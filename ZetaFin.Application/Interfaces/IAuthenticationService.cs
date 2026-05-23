using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IAuthenticationService
{
    Task<(bool Success, string? Error, AuthResponseDto? Data)> RegisterAsync(
        RegisterDto dto,
        string? ipAddress = null,
        string? userAgent = null);

    Task<(bool Success, string? Error, AuthResponseDto? Data)> LoginAsync(
        LoginDto dto,
        string? deviceName = null,
        string? deviceType = null,
        string? ipAddress = null,
        string? userAgent = null);

    Task<(bool Success, string? Error, AuthResponseDto? Data)> RefreshTokenAsync(
        RefreshTokenDto dto,
        string? ipAddress = null,
        string? userAgent = null);

    Task<bool> LogoutAsync(Guid userId, Guid? sessionId = null);

    Task<bool> LogoutAllAsync(Guid userId);

    Task<(bool Success, string? Error)> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);

    Task<(bool Success, string? Error)> ForgotPasswordAsync(string email, string resetUrl);

    Task<(bool Success, string? Error)> ResetPasswordAsync(ResetPasswordDto dto);

    Task<(bool Success, string? Error)> ConfirmEmailAsync(string email, string token);

    // Sessões
    Task<IEnumerable<UserSessionDto>> GetActiveSessionsAsync(Guid userId);

    Task<bool> TerminateSessionAsync(Guid userId, Guid sessionId);

    Task<bool> TerminateAllSessionsAsync(Guid userId);
}

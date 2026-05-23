using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;

    public AuthenticationService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserSessionRepository userSessionRepository,
        IAuditLogService auditLogService,
        IJwtTokenService jwtTokenService,
        IPasswordService passwordService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Data)> RegisterAsync(
        RegisterDto dto,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            // Validar DTO
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, "Name, Email and Password are required", null);
            }

            if (dto.Password != dto.PasswordConfirmation)
            {
                return (false, "Passwords do not match", null);
            }

            // Validar força da senha
            var (isValid, errors) = _passwordService.ValidatePasswordStrength(dto.Password);
            if (!isValid)
            {
                return (false, $"Password validation failed: {string.Join(", ", errors)}", null);
            }

            // Verificar se email já existe
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                await _auditLogService.LogAuthenticationAsync(
                    null, "register_attempt", "failure", ipAddress ?? "unknown",
                    userAgent ?? "unknown", "Email already exists");
                return (false, "Email already registered", null);
            }

            // Criar novo usuário
            var user = new User(dto.Name, dto.Email, dto.Password);
            await _userRepository.AddAsync(user);

            // Log de sucesso
            await _auditLogService.LogAuthenticationAsync(
                user.Id, "register", "success", ipAddress ?? "unknown", userAgent ?? "unknown");

            // Gerar tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.Name, user.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Criar refresh token
            var refreshTokenEntity = new RefreshToken(
                user.Id,
                refreshToken,
                "Web Registration",
                "Web",
                ipAddress ?? "unknown");

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Criar sessão
            var session = new UserSession(
                user.Id,
                "Web Registration",
                "Web",
                ipAddress ?? "unknown",
                userAgent ?? "unknown");
            session.SetRefreshTokenId(refreshTokenEntity.Id);
            await _userSessionRepository.AddAsync(session);

            return (true, null, new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessTokenExpiresIn = 900, // 15 minutos
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Data)> LoginAsync(
        LoginDto dto,
        string? deviceName = null,
        string? deviceType = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, "Email and Password are required", null);
            }

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                await _auditLogService.LogAuthenticationAsync(
                    null, "login", "failure", ipAddress ?? "unknown",
                    userAgent ?? "unknown", "User not found");
                return (false, "Invalid credentials", null);
            }

            if (!_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                await _auditLogService.LogAuthenticationAsync(
                    user.Id, "login", "failure", ipAddress ?? "unknown",
                    userAgent ?? "unknown", "Invalid password");
                return (false, "Invalid credentials", null);
            }

            // Gerar tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.Name, user.Role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Criar refresh token
            var refreshTokenEntity = new RefreshToken(
                user.Id,
                refreshToken,
                deviceName ?? "Web",
                deviceType ?? "Web",
                ipAddress ?? "unknown");

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Criar sessão
            var session = new UserSession(
                user.Id,
                deviceName ?? "Web",
                deviceType ?? "Web",
                ipAddress ?? "unknown",
                userAgent ?? "unknown");
            session.SetRefreshTokenId(refreshTokenEntity.Id);
            await _userSessionRepository.AddAsync(session);

            // Log
            await _auditLogService.LogAuthenticationAsync(
                user.Id, "login", "success", ipAddress ?? "unknown", userAgent ?? "unknown");

            return (true, null, new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessTokenExpiresIn = 900,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string? Error, AuthResponseDto? Data)> RefreshTokenAsync(
        RefreshTokenDto dto,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                return (false, "Invalid or expired refresh token", null);
            }

            var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                return (false, "User not found", null);
            }

            // Gerar novo access token
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, user.Name, user.Role);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Revogar token antigo
            refreshTokenEntity.Revoke("Token refreshed");
            await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);

            // Criar novo refresh token
            var newRefreshTokenEntity = new RefreshToken(
                user.Id,
                newRefreshToken,
                dto.DeviceName ?? "Web",
                "Web",
                ipAddress ?? "unknown");

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            // Atualizar sessão
            var session = await _userSessionRepository.GetByIdAsync(refreshTokenEntity.Id);
            if (session != null)
            {
                session.UpdateLastAccess();
                session.SetRefreshTokenId(newRefreshTokenEntity.Id);
                await _userSessionRepository.UpdateAsync(session);
            }

            return (true, null, new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessTokenExpiresIn = 900,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            return (false, $"Token refresh failed: {ex.Message}", null);
        }
    }

    public async Task<bool> LogoutAsync(Guid userId, Guid? sessionId = null)
    {
        try
        {
            if (sessionId.HasValue)
            {
                var session = await _userSessionRepository.GetByIdAsync(sessionId.Value);
                if (session != null)
                {
                    session.Terminate();
                    await _userSessionRepository.UpdateAsync(session);
                }
            }

            await _auditLogService.LogAuthenticationAsync(
                userId, "logout", "success", "unknown", "unknown");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LogoutAllAsync(Guid userId)
    {
        try
        {
            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, "Logout all");
            await _userSessionRepository.TerminateAllByUserIdAsync(userId);

            await _auditLogService.LogAuthenticationAsync(
                userId, "logout_all", "success", "unknown", "unknown");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (!_passwordService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            {
                await _auditLogService.LogAuthenticationAsync(
                    userId, "change_password", "failure", "unknown", "unknown", "Invalid current password");
                return (false, "Current password is incorrect");
            }

            if (dto.NewPassword != dto.NewPasswordConfirmation)
            {
                return (false, "New passwords do not match");
            }

            var (isValid, errors) = _passwordService.ValidatePasswordStrength(dto.NewPassword);
            if (!isValid)
            {
                return (false, $"Password validation failed: {string.Join(", ", errors)}");
            }

            user.UpdatePassword(_passwordService.HashPassword(dto.NewPassword));
            await _userRepository.UpdateAsync(user);

            await _auditLogService.LogAuthenticationAsync(
                userId, "change_password", "success", "unknown", "unknown");

            // Revogar todos os tokens
            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, "Password changed");

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Change password failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> ForgotPasswordAsync(string email, string resetUrl)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // Não revelar se email existe (LGPD)
                return (true, null);
            }

            // TODO: Gerar token de reset e enviar email
            // Por enquanto, apenas log
            await _auditLogService.LogAuthenticationAsync(
                user.Id, "forgot_password", "success", "unknown", "unknown");

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Forgot password failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return (false, "User not found");
            }

            // TODO: Validar token de reset
            // Por enquanto, apenas atualizar senha
            var (isValid, errors) = _passwordService.ValidatePasswordStrength(dto.NewPassword);
            if (!isValid)
            {
                return (false, $"Password validation failed: {string.Join(", ", errors)}");
            }

            if (dto.NewPassword != dto.NewPasswordConfirmation)
            {
                return (false, "Passwords do not match");
            }

            user.UpdatePassword(_passwordService.HashPassword(dto.NewPassword));
            await _userRepository.UpdateAsync(user);

            await _auditLogService.LogAuthenticationAsync(
                user.Id, "reset_password", "success", "unknown", "unknown");

            // Revogar todos os tokens
            await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id, "Password reset");

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Reset password failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> ConfirmEmailAsync(string email, string token)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, "User not found");
            }

            // TODO: Validar token de confirmação
            user.ConfirmEmail();
            await _userRepository.UpdateAsync(user);

            await _auditLogService.LogAuthenticationAsync(
                user.Id, "confirm_email", "success", "unknown", "unknown");

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Confirm email failed: {ex.Message}");
        }
    }

    public async Task<IEnumerable<UserSessionDto>> GetActiveSessionsAsync(Guid userId)
    {
        var sessions = await _userSessionRepository.GetActiveByUserIdAsync(userId);
        return sessions.Select(s => new UserSessionDto
        {
            Id = s.Id,
            DeviceName = s.DeviceName,
            DeviceType = s.DeviceType,
            IpAddress = s.IpAddress,
            CreatedAt = s.CreatedAt,
            LastAccessAt = s.LastAccessAt,
            IsActive = s.IsActive
        });
    }

    public async Task<bool> TerminateSessionAsync(Guid userId, Guid sessionId)
    {
        try
        {
            var session = await _userSessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.UserId != userId)
            {
                return false;
            }

            session.Terminate();
            await _userSessionRepository.UpdateAsync(session);

            if (session.RefreshTokenId.HasValue)
            {
                var refreshToken = await _refreshTokenRepository.GetByIdAsync(session.RefreshTokenId.Value);
                if (refreshToken != null)
                {
                    refreshToken.Revoke("Session terminated");
                    await _refreshTokenRepository.UpdateAsync(refreshToken);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TerminateAllSessionsAsync(Guid userId)
    {
        try
        {
            await _userSessionRepository.TerminateAllByUserIdAsync(userId);
            await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, "All sessions terminated");
            return true;
        }
        catch
        {
            return false;
        }
    }
}

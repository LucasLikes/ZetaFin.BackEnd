namespace ZetaFin.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string name, string role, Dictionary<string, object>? customClaims = null);
    string GenerateRefreshToken();
    (bool Valid, string? Error, object? Principal) ValidateToken(string token, bool validateExpiry = true);
}

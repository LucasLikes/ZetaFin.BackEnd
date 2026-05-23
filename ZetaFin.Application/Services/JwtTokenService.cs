using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GenerateAccessToken(
        Guid userId,
        string email,
        string name,
        string role,
        Dictionary<string, object>? customClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.UniqueName, name),
            new Claim(ClaimTypes.Role, role),
            new Claim("type", "access_token")
        };

        // Adicionar claims customizados
        if (customClaims != null)
        {
            foreach (var claim in customClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value.ToString() ?? string.Empty));
            }
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public (bool Valid, string? Error, object? Principal) ValidateToken(string token, bool validateExpiry = true)
    {
        try
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured")));

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = validateExpiry,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return (true, null, principal);
        }
        catch (SecurityTokenExpiredException)
        {
            return (false, "Token expired", null);
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return (false, "Invalid token signature", null);
        }
        catch (Exception ex)
        {
            return (false, $"Token validation failed: {ex.Message}", null);
        }
    }
}

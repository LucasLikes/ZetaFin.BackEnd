using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class WhatsAppAuthService : IWhatsAppAuthService
{
    private readonly IUserWhatsAppRepository _userWhatsAppRepository;
    private readonly IConfiguration _configuration;

    public WhatsAppAuthService(
        IUserWhatsAppRepository userWhatsAppRepository,
        IConfiguration configuration)
    {
        _userWhatsAppRepository = userWhatsAppRepository;
        _configuration = configuration;
    }

    public async Task<WhatsAppAuthResponseDto?> AuthenticateByWhatsAppAsync(string whatsAppNumber)
    {
        var userWhatsApp = await _userWhatsAppRepository.GetByWhatsAppNumberAsync(whatsAppNumber);

        if (userWhatsApp == null)
            return null;

        // Atualizar última mensagem
        userWhatsApp.UpdateLastMessage();
        await _userWhatsAppRepository.UpdateAsync(userWhatsApp);

        // Gerar JWT
        var token = GenerateJwtToken(userWhatsApp.User);

        return new WhatsAppAuthResponseDto
        {
            Token = token,
            UserId = userWhatsApp.UserId,
            UserName = userWhatsApp.User.Name,
            WhatsAppNumber = userWhatsApp.WhatsAppNumber
        };
    }

    public async Task<bool> LinkWhatsAppToUserAsync(Guid userId, string whatsAppNumber)
    {
        // Verificar se já existe
        if (await _userWhatsAppRepository.ExistsAsync(whatsAppNumber))
            return false;

        var userWhatsApp = new UserWhatsApp(userId, whatsAppNumber);
        await _userWhatsAppRepository.AddAsync(userWhatsApp);

        return true;
    }

    public async Task<bool> UnlinkWhatsAppAsync(Guid userId)
    {
        var userWhatsApp = await _userWhatsAppRepository.GetByUserIdAsync(userId);

        if (userWhatsApp == null)
            return false;

        userWhatsApp.Deactivate();
        await _userWhatsAppRepository.UpdateAsync(userWhatsApp);

        return true;
    }

    public async Task<bool> IsWhatsAppLinkedAsync(string whatsAppNumber)
    {
        return await _userWhatsAppRepository.ExistsAsync(whatsAppNumber);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("auth_method", "whatsapp") // Identifica que veio do WhatsApp
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // Token de longa duração para bots
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IWhatsAppAuthService
{
    Task<WhatsAppAuthResponseDto?> AuthenticateByWhatsAppAsync(string whatsAppNumber);
    Task<bool> LinkWhatsAppToUserAsync(Guid userId, string whatsAppNumber);
    Task<bool> UnlinkWhatsAppAsync(Guid userId);
    Task<bool> IsWhatsAppLinkedAsync(string whatsAppNumber);
}
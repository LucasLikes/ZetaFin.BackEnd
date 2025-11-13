using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IUserWhatsAppRepository
{
    Task<UserWhatsApp?> GetByWhatsAppNumberAsync(string whatsAppNumber);
    Task<UserWhatsApp?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserWhatsApp userWhatsApp);
    Task UpdateAsync(UserWhatsApp userWhatsApp);
    Task<bool> ExistsAsync(string whatsAppNumber);
}
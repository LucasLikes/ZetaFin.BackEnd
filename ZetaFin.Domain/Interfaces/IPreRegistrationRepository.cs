using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IPreRegistrationRepository
{
    Task<PreRegistration?> GetByWhatsAppAsync(string whatsApp);
    Task<PreRegistration?> GetByIdAsync(Guid id);
    Task<IEnumerable<PreRegistration>> GetAllAsync();
    Task AddAsync(PreRegistration preRegistration);
    Task SaveChangesAsync();
}

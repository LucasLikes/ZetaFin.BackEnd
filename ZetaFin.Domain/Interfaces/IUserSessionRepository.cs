using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession session);
    Task<UserSession?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserSession>> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<UserSession>> GetAllByUserIdAsync(Guid userId);
    Task UpdateAsync(UserSession session);
    Task DeleteAsync(Guid id);
    Task<int> TerminateAllByUserIdAsync(Guid userId);
}

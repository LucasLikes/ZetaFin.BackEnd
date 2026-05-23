using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int limit = 100);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, int limit = 100);
    Task<IEnumerable<AuditLog>> GetRecentAsync(int days = 7, int limit = 100);
}

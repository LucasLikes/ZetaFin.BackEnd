using ZetaFin.Domain.Entities;

namespace ZetaFin.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAuthenticationAsync(
        Guid? userId,
        string action,
        string status,
        string ipAddress,
        string userAgent,
        string? details = null);

    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(Guid userId, int limit = 100);
}

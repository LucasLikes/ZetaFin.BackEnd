using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
    }

    public async Task LogAuthenticationAsync(
        Guid? userId,
        string action,
        string status,
        string ipAddress,
        string userAgent,
        string? details = null)
    {
        var auditLog = new AuditLog(
            action,
            "Authentication",
            ipAddress,
            userAgent,
            status,
            userId,
            details);

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(Guid userId, int limit = 100)
    {
        return await _auditLogRepository.GetByUserIdAsync(userId, limit);
    }
}

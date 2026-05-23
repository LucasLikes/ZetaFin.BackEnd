using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence;

namespace ZetaFin.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int limit = 100)
    {
        return await _context.AuditLogs
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action, int limit = 100)
    {
        return await _context.AuditLogs
            .Where(al => al.Action == action)
            .OrderByDescending(al => al.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetRecentAsync(int days = 7, int limit = 100)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _context.AuditLogs
            .Where(al => al.CreatedAt >= since)
            .OrderByDescending(al => al.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}

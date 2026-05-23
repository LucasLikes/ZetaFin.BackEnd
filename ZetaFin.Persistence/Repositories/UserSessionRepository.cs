using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence;

namespace ZetaFin.Persistence.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSessionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(UserSession session)
    {
        await _context.UserSessions.AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task<UserSession?> GetByIdAsync(Guid id)
    {
        return await _context.UserSessions.FirstOrDefaultAsync(us => us.Id == id);
    }

    public async Task<IEnumerable<UserSession>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.UserSessions
            .Where(us => us.UserId == userId && us.IsActive)
            .OrderByDescending(us => us.LastAccessAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSession>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.UserSessions
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(UserSession session)
    {
        _context.UserSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await GetByIdAsync(id);
        if (session != null)
        {
            _context.UserSessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> TerminateAllByUserIdAsync(Guid userId)
    {
        var sessions = await _context.UserSessions
            .Where(us => us.UserId == userId && us.IsActive)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.Terminate();
        }

        await _context.SaveChangesAsync();
        return sessions.Count;
    }
}

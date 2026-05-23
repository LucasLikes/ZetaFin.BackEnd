using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence;

namespace ZetaFin.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByIdAsync(Guid id)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var refreshToken = await GetByIdAsync(id);
        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> RevokeAllByUserIdAsync(Guid userId, string reason = "Logout")
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke(reason);
        }

        await _context.SaveChangesAsync();
        return tokens.Count;
    }

    public async Task<int> RevokeExpiredAsync()
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt <= DateTime.UtcNow && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokeByExpiration();
        }

        await _context.SaveChangesAsync();
        return tokens.Count;
    }
}

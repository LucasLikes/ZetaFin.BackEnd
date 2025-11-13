using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Persistence.Repositories;

public class UserWhatsAppRepository : IUserWhatsAppRepository
{
    private readonly ApplicationDbContext _context;

    public UserWhatsAppRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserWhatsApp?> GetByWhatsAppNumberAsync(string whatsAppNumber)
    {
        return await _context.UserWhatsApps
            .Include(uw => uw.User)
            .FirstOrDefaultAsync(uw => uw.WhatsAppNumber == whatsAppNumber && uw.IsActive);
    }

    public async Task<UserWhatsApp?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserWhatsApps
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.IsActive);
    }

    public async Task AddAsync(UserWhatsApp userWhatsApp)
    {
        await _context.UserWhatsApps.AddAsync(userWhatsApp);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserWhatsApp userWhatsApp)
    {
        _context.UserWhatsApps.Update(userWhatsApp);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string whatsAppNumber)
    {
        return await _context.UserWhatsApps
            .AnyAsync(uw => uw.WhatsAppNumber == whatsAppNumber);
    }
}
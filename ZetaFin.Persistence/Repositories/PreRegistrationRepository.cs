using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Persistence.Repositories;

public class PreRegistrationRepository : IPreRegistrationRepository
{
    private readonly ApplicationDbContext _context;

    public PreRegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PreRegistration?> GetByWhatsAppAsync(string whatsApp)
        => await _context.PreRegistrations
            .FirstOrDefaultAsync(p => p.WhatsApp == whatsApp);

    public async Task<PreRegistration?> GetByIdAsync(Guid id)
        => await _context.PreRegistrations.FindAsync(id);

    public async Task<IEnumerable<PreRegistration>> GetAllAsync()
        => await _context.PreRegistrations
            .OrderByDescending(p => p.DataCadastro)
            .ToListAsync();

    public async Task AddAsync(PreRegistration preRegistration)
        => await _context.PreRegistrations.AddAsync(preRegistration);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
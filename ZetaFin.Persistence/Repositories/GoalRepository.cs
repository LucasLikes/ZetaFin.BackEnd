using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence.Context;

namespace ZetaFin.Persistence.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly ApplicationDbContext _context;

    public GoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Goal goal)
    {
        await _context.Goals.AddAsync(goal);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Goal>> GetAllAsync()
    {
        return await _context.Goals.ToListAsync();
    }

    public async Task<Goal?> GetByIdAsync(Guid id)
    {
        return await _context.Goals.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

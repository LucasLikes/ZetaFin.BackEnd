using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;
using ZetaFin.Persistence.Context;

namespace ZetaFin.Persistence.Repositories;

public class DepositRepository : IDepositRepository
{
    private readonly ApplicationDbContext _context;

    public DepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Deposit deposit)
    {
        await _context.Deposits.AddAsync(deposit);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Deposit>> GetByGoalIdAsync(Guid goalId)
    {
        return await _context.Deposits
            .Where(d => d.GoalId == goalId)
            .ToListAsync();
    }
}

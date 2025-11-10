using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Persistence.Repositories;

public class UserGoalRepository : IUserGoalRepository
{
    private readonly ApplicationDbContext _context;

    public UserGoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserGoal userGoal)
    {
        await _context.UserGoals.AddAsync(userGoal);
    }

    public async Task<UserGoal?> GetByUserIdAndGoalIdAsync(Guid userId, Guid goalId)
    {
        return await _context.UserGoals.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GoalId == goalId);
    }

    public async Task<IEnumerable<UserGoal>> GetByGoalIdAsync(Guid goalId)
    {
        return await _context.UserGoals.Where(ug => ug.GoalId == goalId).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

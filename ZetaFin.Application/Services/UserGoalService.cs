using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ZetaFin.Application.Services;

public class UserGoalService : IUserGoalService
{
    private readonly ApplicationDbContext _context;

    public UserGoalService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateUserGoalAsync(CreateUserGoalDto dto)
    {
        var userGoal = new UserGoal(dto.UserId, dto.GoalId, dto.CustomMonthlyTarget);
        _context.UserGoals.Add(userGoal);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMonthlyTargetAsync(Guid userId, Guid goalId, decimal? customMonthlyTarget)
    {
        var userGoal = await _context.UserGoals
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GoalId == goalId);

        if (userGoal == null)
            throw new Exception("UserGoal not found");

        userGoal.UpdateCustomMonthlyTarget(customMonthlyTarget);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserGoalDto>> GetUserGoalsByGoalIdAsync(Guid goalId)
    {
        var userGoals = await _context.UserGoals
            .Where(ug => ug.GoalId == goalId)
            .ToListAsync();

        return userGoals.Select(ug => new UserGoalDto
        {
            UserId = ug.UserId,
            GoalId = ug.GoalId,
            CustomMonthlyTarget = ug.CustomMonthlyTarget
        });
    }
}

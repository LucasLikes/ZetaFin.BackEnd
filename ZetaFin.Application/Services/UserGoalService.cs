using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class UserGoalService : IUserGoalService
{
    private readonly IUserGoalRepository _userGoalRepository;

    public UserGoalService(IUserGoalRepository userGoalRepository)
    {
        _userGoalRepository = userGoalRepository;
    }

    public async Task CreateUserGoalAsync(CreateUserGoalDto dto)
    {
        var userGoal = new UserGoal(dto.UserId, dto.GoalId, dto.CustomMonthlyTarget);
        await _userGoalRepository.AddAsync(userGoal);
        await _userGoalRepository.SaveChangesAsync();
    }

    public async Task UpdateMonthlyTargetAsync(Guid userId, Guid goalId, decimal? customMonthlyTarget)
    {
        var userGoal = await _userGoalRepository.GetByUserIdAndGoalIdAsync(userId, goalId);
        if (userGoal == null)
            throw new Exception("UserGoal not found");

        userGoal.UpdateCustomMonthlyTarget(customMonthlyTarget);
        await _userGoalRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserGoalDto>> GetUserGoalsByGoalIdAsync(Guid goalId)
    {
        var userGoals = await _userGoalRepository.GetByGoalIdAsync(goalId);

        return userGoals.Select(ug => new UserGoalDto
        {
            UserId = ug.UserId,
            GoalId = ug.GoalId,
            CustomMonthlyTarget = ug.CustomMonthlyTarget
        });
    }
}

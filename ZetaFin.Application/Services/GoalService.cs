using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;

    public GoalService(IGoalRepository goalRepository)
    {
        _goalRepository = goalRepository;
    }

    public async Task<GoalDto> CreateGoalAsync(CreateGoalDto dto)
    {
        var goal = new Goal(dto.Description, dto.TargetAmount, dto.TargetDate);
        await _goalRepository.AddAsync(goal);
        return MapToDto(goal);
    }

    public async Task<IEnumerable<GoalDto>> GetGoalsAsync()
    {
        var goals = await _goalRepository.GetAllAsync();
        return goals.Select(MapToDto);
    }

    public async Task<GoalDto?> GetGoalByIdAsync(Guid id)
    {
        var goal = await _goalRepository.GetByIdAsync(id);
        return goal == null ? null : MapToDto(goal);
    }

    private GoalDto MapToDto(Goal goal)
    {
        return new GoalDto
        {
            Id = goal.Id,
            Description = goal.Description,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            CreatedAt = goal.CreatedAt,
            TargetDate = goal.TargetDate
        };
    }

    public async Task AddDepositToGoalAsync(Guid goalId, decimal amount)
    {
        var goal = await _goalRepository.GetByIdAsync(goalId);
        if (goal == null)
            throw new ArgumentException("Goal not found.");

        goal.AddAmount(amount);

        await _goalRepository.SaveChangesAsync();
    }
}

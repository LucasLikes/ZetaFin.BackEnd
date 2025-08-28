using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IGoalService
{
    Task<GoalDto> CreateGoalAsync(CreateGoalDto dto);
    Task<IEnumerable<GoalDto>> GetGoalsAsync();
    Task<GoalDto?> GetGoalByIdAsync(Guid id);
    Task AddDepositToGoalAsync(Guid goalId, decimal amount);
}


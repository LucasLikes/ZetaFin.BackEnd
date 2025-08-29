using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IUserGoalService
{
    Task CreateUserGoalAsync(CreateUserGoalDto dto);
    Task UpdateMonthlyTargetAsync(Guid userId, Guid goalId, decimal? customMonthlyTarget);
    Task<IEnumerable<UserGoalDto>> GetUserGoalsByGoalIdAsync(Guid goalId);
}

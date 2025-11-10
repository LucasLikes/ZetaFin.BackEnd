using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;
public interface IUserGoalRepository
{
    Task AddAsync(UserGoal userGoal);
    Task<UserGoal?> GetByUserIdAndGoalIdAsync(Guid userId, Guid goalId);
    Task<IEnumerable<UserGoal>> GetByGoalIdAsync(Guid goalId);
    Task SaveChangesAsync();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Domain.Entities;

public class UserGoal
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid GoalId { get; private set; }
    public Goal Goal { get; private set; }
    public decimal? CustomMonthlyTarget { get; private set; }

    public UserGoal(Guid userId, Guid goalId, decimal? customMonthlyTarget = null)
    {
        UserId = userId;
        GoalId = goalId;
        CustomMonthlyTarget = customMonthlyTarget;
    }

    public void UpdateCustomMonthlyTarget(decimal? value)
    {
        CustomMonthlyTarget = value;
    }
}

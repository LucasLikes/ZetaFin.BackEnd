using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Application.DTOs;

public class UserGoalDto
{
    public Guid UserId { get; set; }
    public Guid GoalId { get; set; }
    public decimal? CustomMonthlyTarget { get; set; }
}

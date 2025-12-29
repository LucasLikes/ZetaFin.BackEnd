using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Application.DTOs;

public class GoalDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? TargetDate { get; set; } = DateTime.UtcNow;
}

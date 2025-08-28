using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Domain.Interfaces;

public interface IGoalRepository
{
    Task AddAsync(Entities.Goal goal);
    Task<IEnumerable<Entities.Goal>> GetAllAsync();
    Task<Entities.Goal?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}

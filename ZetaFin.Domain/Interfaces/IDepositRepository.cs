using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IDepositRepository
{
    Task AddAsync(Deposit deposit);
    Task<IEnumerable<Deposit>> GetByGoalIdAsync(Guid goalId);
}

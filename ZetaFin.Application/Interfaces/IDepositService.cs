using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

public interface IDepositService
{
    Task<DepositDto> AddDepositAsync(CreateDepositDto dto);
    Task<IEnumerable<DepositDto>> GetDepositsByGoalIdAsync(Guid goalId);
}

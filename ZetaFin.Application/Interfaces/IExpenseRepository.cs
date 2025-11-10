using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Application.Interfaces
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetByUserIdAsync(string userId);
        Task<Expense?> GetByIdAsync(Guid id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
    }
}

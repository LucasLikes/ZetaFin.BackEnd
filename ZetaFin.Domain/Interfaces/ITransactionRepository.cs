using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetFilteredAsync(
        Guid userId,
        TransactionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? category = null,
        ExpenseType? expenseType = null,
        int skip = 0,
        int take = 20);
    Task<int> CountFilteredAsync(
        Guid userId,
        TransactionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? category = null,
        ExpenseType? expenseType = null);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
    Task<decimal> GetTotalIncomeAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetTotalExpenseAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Dictionary<string, decimal>> GetIncomeByCategoryAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Dictionary<string, decimal>> GetExpenseByCategoryAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Dictionary<string, decimal>> GetExpenseByTypeAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ExistsAsync(Guid id);
}
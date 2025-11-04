using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;
public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId);
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<ExpenseDto> CreateAsync(CreateExpenseRequest request);
    Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<Dictionary<string, decimal>> GetSummaryByCategoryAsync(string userId);
}
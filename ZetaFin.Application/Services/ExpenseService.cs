using AutoMapper;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Constants;

namespace ZetaFin.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;
    private readonly IMapper _mapper;

    public ExpenseService(IExpenseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesByUserAsync(string userId)
    {
        var expenses = await _repository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ExpenseDto>>(expenses);
    }

    public async Task<ExpenseDto?> GetByIdAsync(Guid id)
    {
        var expense = await _repository.GetByIdAsync(id);
        return expense == null ? null : _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseRequest request)
    {
        var expense = new Expense
        {
            UserId = request.UserId,
            Name = request.Name,
            Value = request.Value,
            Category = request.Category,
            Date = DateTime.UtcNow,
            DueDate = request.DueDate
        };

        await _repository.AddAsync(expense);
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseRequest request)
    {
        var expense = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Expense not found.");

        expense.Name = request.Name;
        expense.Value = request.Value;
        expense.Category = request.Category;
        expense.DueDate = request.DueDate;

        await _repository.UpdateAsync(expense);
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null) return false;

        await _repository.DeleteAsync(expense);
        return true;
    }

    public async Task<Dictionary<string, decimal>> GetSummaryByCategoryAsync(string userId)
    {
        var expenses = await _repository.GetByUserIdAsync(userId);

        // Inicializa com categorias zeradas
        var result = ExpenseCategories.GetDefaultSummary();

        // Se não houver despesas, retorna as categorias zeradas
        if (!expenses.Any())
        {
            return result;
        }

        // Agrupa despesas por categoria e soma valores
        var summaryFromDb = expenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Value));

        // Mescla valores reais nas categorias padrão
        foreach (var category in summaryFromDb)
        {
            // Se categoria existe no padrão, atualiza
            if (result.ContainsKey(category.Key))
            {
                result[category.Key] = category.Value;
            }
            // Se categoria customizada, adiciona também
            else
            {
                result[category.Key] = category.Value;
            }
        }

        return result;
    }
}
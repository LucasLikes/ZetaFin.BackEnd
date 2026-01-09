using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ... (métodos existentes permanecem os mesmos) ...

    // ===== MÉTODOS AJUSTADOS PARA SEMPRE RETORNAR ESTRUTURAS VÁLIDAS =====

    public async Task<decimal> GetTotalIncomeAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Income);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        // Se não houver transações, retorna 0 ao invés de null
        var total = await query.SumAsync(t => (decimal?)t.Value);
        return total ?? 0m;
    }

    public async Task<decimal> GetTotalExpenseAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        // Se não houver transações, retorna 0 ao invés de null
        var total = await query.SumAsync(t => (decimal?)t.Value);
        return total ?? 0m;
    }

    public async Task<Dictionary<string, decimal>> GetIncomeByCategoryAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Income);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var results = await query
            .GroupBy(t => t.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Value) })
            .ToListAsync();

        // Sempre retorna um dicionário, mesmo vazio
        return results.Any()
            ? results.ToDictionary(x => x.Category, x => x.Total)
            : new Dictionary<string, decimal>();
    }

    public async Task<Dictionary<string, decimal>> GetExpenseByCategoryAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var results = await query
            .GroupBy(t => t.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Value) })
            .ToListAsync();

        // Sempre retorna um dicionário, mesmo vazio
        return results.Any()
            ? results.ToDictionary(x => x.Category, x => x.Total)
            : new Dictionary<string, decimal>();
    }

    public async Task<Dictionary<string, decimal>> GetExpenseByTypeAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.ExpenseType != null);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var results = await query
            .GroupBy(t => t.ExpenseType!.Value)
            .Select(g => new { Type = g.Key.ToString().ToLower(), Total = g.Sum(t => t.Value) })
            .ToListAsync();

        // Sempre retorna um dicionário, mesmo vazio
        // Se vazio, inicializa com as chaves esperadas pelo frontend
        if (!results.Any())
        {
            return new Dictionary<string, decimal>
            {
                { "fixas", 0m },
                { "variaveis", 0m },
                { "desnecessarios", 0m }
            };
        }

        return results.ToDictionary(x => x.Type, x => x.Total);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions
            .Include(t => t.Receipt)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetFilteredAsync(
        Guid userId,
        TransactionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? category = null,
        ExpenseType? expenseType = null,
        int skip = 0,
        int take = 20)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        if (expenseType.HasValue)
            query = query.Where(t => t.ExpenseType == expenseType.Value);

        return await query
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountFilteredAsync(
        Guid userId,
        TransactionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? category = null,
        ExpenseType? expenseType = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        if (expenseType.HasValue)
            query = query.Where(t => t.ExpenseType == expenseType.Value);

        return await query.CountAsync();
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Transactions.AnyAsync(t => t.Id == id);
    }
}
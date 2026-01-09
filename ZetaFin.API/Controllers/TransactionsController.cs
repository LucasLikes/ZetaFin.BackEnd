using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
    }

    public async Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionDto dto)
    {
        // Validar se o usuário existe
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuário não encontrado");

        // IMPORTANTE: Garantir que a data está em UTC
        var dateUtc = dto.Date.Kind == DateTimeKind.Utc
            ? dto.Date
            : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);

        var transaction = new Transaction(
            userId,
            dto.Type,
            dto.Value,
            dto.Description,
            dto.Category,
            dateUtc, // Usa data em UTC
            dto.ExpenseType
        );

        await _transactionRepository.AddAsync(transaction);

        return MapToDto(transaction);
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        return transaction == null ? null : MapToDto(transaction);
    }

    public async Task<TransactionListDto> GetFilteredAsync(
        Guid userId,
        TransactionQueryDto query)
    {
        var skip = (query.Page - 1) * query.Limit;

        // CORREÇÃO: Converter datas para UTC antes de consultar
        DateTime? startDateUtc = ConvertToUtc(query.StartDate);
        DateTime? endDateUtc = ConvertToUtc(query.EndDate);

        var transactions = await _transactionRepository.GetFilteredAsync(
            userId,
            query.Type,
            startDateUtc,
            endDateUtc,
            query.Category,
            query.ExpenseType,
            skip,
            query.Limit
        );

        var totalItems = await _transactionRepository.CountFilteredAsync(
            userId,
            query.Type,
            startDateUtc,
            endDateUtc,
            query.Category,
            query.ExpenseType
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)query.Limit);

        // Calcular resumo
        var totalIncome = await _transactionRepository.GetTotalIncomeAsync(
            userId, startDateUtc, endDateUtc);
        var totalExpense = await _transactionRepository.GetTotalExpenseAsync(
            userId, startDateUtc, endDateUtc);

        return new TransactionListDto
        {
            Transactions = transactions.Select(MapToDto).ToList(),
            Pagination = new PaginationDto
            {
                CurrentPage = query.Page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                ItemsPerPage = query.Limit
            },
            Summary = new TransactionSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                SavingsRate = totalIncome > 0 ? (double)((totalIncome - totalExpense) / totalIncome) : 0
            }
        };
    }

    public async Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
            throw new Exception("Transação não encontrada");

        // Converter data para UTC
        var dateUtc = ConvertToUtc(dto.Date) ?? DateTime.UtcNow;

        transaction.Update(dto.Value, dto.Description, dto.Category, dateUtc);
        await _transactionRepository.UpdateAsync(transaction);

        return MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
            return false;

        await _transactionRepository.DeleteAsync(transaction);
        return true;
    }

    public async Task<DetailedSummaryDto> GetSummaryAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        // CORREÇÃO: Converter datas para UTC
        DateTime? startDateUtc = ConvertToUtc(startDate);
        DateTime? endDateUtc = ConvertToUtc(endDate);

        var totalIncome = await _transactionRepository.GetTotalIncomeAsync(userId, startDateUtc, endDateUtc);
        var totalExpense = await _transactionRepository.GetTotalExpenseAsync(userId, startDateUtc, endDateUtc);
        var incomeByCategory = await _transactionRepository.GetIncomeByCategoryAsync(userId, startDateUtc, endDateUtc);
        var expenseByCategory = await _transactionRepository.GetExpenseByCategoryAsync(userId, startDateUtc, endDateUtc);
        var expenseByType = await _transactionRepository.GetExpenseByTypeAsync(userId, startDateUtc, endDateUtc);

        // Contar transações
        var incomeCount = await _transactionRepository.CountFilteredAsync(
            userId, TransactionType.Income, startDateUtc, endDateUtc);
        var expenseCount = await _transactionRepository.CountFilteredAsync(
            userId, TransactionType.Expense, startDateUtc, endDateUtc);

        var balance = totalIncome - totalExpense;
        var savingsRate = totalIncome > 0 ? (double)(balance / totalIncome) : 0;

        return new DetailedSummaryDto
        {
            Period = new PeriodDto
            {
                StartDate = startDateUtc ?? DateTime.MinValue,
                EndDate = endDateUtc ?? DateTime.MaxValue
            },
            Income = new IncomeSummaryDto
            {
                Total = totalIncome,
                Count = incomeCount,
                ByCategory = incomeByCategory
            },
            Expense = new ExpenseSummaryDto
            {
                Total = totalExpense,
                Count = expenseCount,
                ByCategory = expenseByCategory,
                ByType = expenseByType
            },
            Balance = balance,
            SavingsRate = savingsRate
        };
    }

    // ====================================================
    // MÉTODO AUXILIAR PARA CONVERTER PARA UTC
    // ====================================================
    private DateTime? ConvertToUtc(DateTime? date)
    {
        if (!date.HasValue)
            return null;

        // Se já está em UTC, retorna direto
        if (date.Value.Kind == DateTimeKind.Utc)
            return date.Value;

        // Se está em Local, converte para UTC
        if (date.Value.Kind == DateTimeKind.Local)
            return date.Value.ToUniversalTime();

        // Se é Unspecified, assume como UTC
        return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Type = transaction.Type.ToString().ToLower(),
            Value = transaction.Value,
            Description = transaction.Description,
            Category = transaction.Category,
            ExpenseType = transaction.ExpenseType?.ToString().ToLower(),
            Date = transaction.Date,
            HasReceipt = transaction.HasReceipt,
            ReceiptUrl = transaction.ReceiptUrl,
            ReceiptOcrData = transaction.ReceiptOcrData != null
                ? JsonSerializer.Deserialize<object>(transaction.ReceiptOcrData)
                : null,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}
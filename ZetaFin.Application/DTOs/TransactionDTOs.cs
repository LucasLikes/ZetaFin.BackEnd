using System;
using System.ComponentModel.DataAnnotations;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Application.DTOs;

// DTO para criar transação
public class CreateTransactionDto
{
    [Required(ErrorMessage = "Tipo de transação é obrigatório")]
    public TransactionType Type { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Value { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatória")]
    [MaxLength(500, ErrorMessage = "Descrição não pode exceder 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    [MaxLength(100, ErrorMessage = "Categoria não pode exceder 100 caracteres")]
    public string Category { get; set; } = string.Empty;

    public ExpenseType? ExpenseType { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    public bool HasReceipt { get; set; }
}

// DTO para atualizar transação
public class UpdateTransactionDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}

// DTO de resposta para transação
public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ExpenseType { get; set; }
    public DateTime Date { get; set; }
    public bool HasReceipt { get; set; }
    public string? ReceiptUrl { get; set; }
    public object? ReceiptOcrData { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// DTO para listagem com paginação
public class TransactionListDto
{
    public List<TransactionDto> Transactions { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
    public TransactionSummaryDto Summary { get; set; } = new();
}

public class PaginationDto
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; }
}

// DTO para resumo financeiro
public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public double SavingsRate { get; set; }
}

// DTO para resumo detalhado
public class DetailedSummaryDto
{
    public PeriodDto Period { get; set; } = new();
    public IncomeSummaryDto Income { get; set; } = new();
    public ExpenseSummaryDto Expense { get; set; } = new();
    public decimal Balance { get; set; }
    public double SavingsRate { get; set; }
}

public class PeriodDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class IncomeSummaryDto
{
    public decimal Total { get; set; }
    public int Count { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
}

public class ExpenseSummaryDto
{
    public decimal Total { get; set; }
    public int Count { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
    public Dictionary<string, decimal> ByType { get; set; } = new();
}

// DTO para filtros de consulta
public class TransactionQueryDto
{
    public TransactionType? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Category { get; set; }
    public ExpenseType? ExpenseType { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}
using System;
using ZetaFin.Domain.Common;

namespace ZetaFin.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Value { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public ExpenseType? ExpenseType { get; private set; }
    public DateTime Date { get; private set; }
    public bool HasReceipt { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public string? ReceiptOcrData { get; private set; } // JSON serializado

    // Relacionamento
    public Receipt? Receipt { get; private set; }

    // Construtor para EF Core
    private Transaction() { }

    public Transaction(
        Guid userId,
        TransactionType type,
        decimal value,
        string description,
        string category,
        DateTime date,
        ExpenseType? expenseType = null)
    {
        ValidateTransaction(type, value, description, category, date, expenseType);

        UserId = userId;
        Type = type;
        Value = value;
        Description = description;
        Category = category;
        Date = date;
        ExpenseType = expenseType;
        HasReceipt = false;
    }

    private void ValidateTransaction(
        TransactionType type,
        decimal value,
        string description,
        string category,
        DateTime date,
        ExpenseType? expenseType)
    {
        if (value <= 0)
            throw new ArgumentException("O valor deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição é obrigatória");

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("A categoria é obrigatória");

        if (type == TransactionType.Expense && date > DateTime.UtcNow.Date)
            throw new ArgumentException("Despesas não podem ter data futura");

        if (type == TransactionType.Expense && !expenseType.HasValue)
            throw new ArgumentException("Tipo de despesa é obrigatório para despesas");

        if (type == TransactionType.Income && expenseType.HasValue)
            throw new ArgumentException("Receitas não podem ter tipo de despesa");
    }

    public void Update(decimal value, string description, string category, DateTime date)
    {
        if (value <= 0)
            throw new ArgumentException("O valor deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição é obrigatória");

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("A categoria é obrigatória");

        Value = value;
        Description = description;
        Category = category;
        Date = date;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachReceipt(string receiptUrl, string? ocrData = null)
    {
        HasReceipt = true;
        ReceiptUrl = receiptUrl;
        ReceiptOcrData = ocrData;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOcrData(string ocrData)
    {
        ReceiptOcrData = ocrData;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum TransactionType
{
    Income,
    Expense
}

public enum ExpenseType
{
    Fixas,
    Variaveis,
    Desnecessarios
}
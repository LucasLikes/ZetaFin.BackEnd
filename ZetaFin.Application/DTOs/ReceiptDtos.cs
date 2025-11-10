using System;
using System.Collections.Generic;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Application.DTOs;
public class ReceiptDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public bool OcrProcessed { get; set; }
    public OcrDataDto? OcrData { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OcrDataDto
{
    public string? MerchantName { get; set; }
    public DateTime? ExtractedDate { get; set; }
    public decimal? ExtractedValue { get; set; }
    public string? Currency { get; set; }
    public List<OcrItemDto>? Items { get; set; }

    // Adiciona confidence
    public double? Confidence { get; set; }
}

public class CreateTransactionFromOcrDto
{
    public string? Description { get; set; }
    public string? Category { get; set; }
    public ExpenseType? ExpenseType { get; set; }
}

public class ReceiptWithTransactionDto
{
    public ReceiptDto Receipt { get; set; } = new();
    public TransactionDto Transaction { get; set; } = new(); // Usa o que já está em TransactionDTOs.cs
}

public class OcrItemDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

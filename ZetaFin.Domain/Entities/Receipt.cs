using System;
using ZetaFin.Domain.Common;

namespace ZetaFin.Domain.Entities;

public class Receipt : BaseEntity
{
    public Guid? TransactionId { get; private set; }
    public Guid UserId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string MimeType { get; private set; } = string.Empty;
    public bool OcrProcessed { get; private set; }
    public string? OcrDataJson { get; private set; } // JSON serializado

    // Relacionamentos
    public Transaction? Transaction { get; private set; }
    public User? User { get; private set; }

    // Construtor para EF Core
    private Receipt() { }

    public Receipt(
        Guid userId,
        string fileName,
        string fileUrl,
        long fileSize,
        string mimeType,
        Guid? transactionId = null)
    {
        ValidateReceipt(fileName, fileUrl, fileSize, mimeType);

        Id = Guid.NewGuid();
        UserId = userId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileSize = fileSize;
        MimeType = mimeType;
        TransactionId = transactionId;
        OcrProcessed = false;
        CreatedAt = DateTime.UtcNow;
    }

    private void ValidateReceipt(string fileName, string fileUrl, long fileSize, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Nome do arquivo é obrigatório");

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("URL do arquivo é obrigatória");

        if (fileSize <= 0)
            throw new ArgumentException("Tamanho do arquivo inválido");

        if (fileSize > 10_485_760) // 10MB
            throw new ArgumentException("Arquivo excede o limite de 10MB");

        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!Array.Exists(allowedTypes, t => t.Equals(mimeType, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Formato de arquivo não suportado. Use JPG, PNG ou PDF");
    }

    public void MarkAsProcessed(string ocrDataJson)
    {
        OcrProcessed = true;
        OcrDataJson = ocrDataJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToTransaction(Guid transactionId)
    {
        TransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOcrData(string ocrDataJson)
    {
        OcrDataJson = ocrDataJson;
        OcrProcessed = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
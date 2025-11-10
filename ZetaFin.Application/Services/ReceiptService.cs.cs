using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStorageService _storageService;
    private readonly IOcrService _ocrService;

    public ReceiptService(
        IReceiptRepository receiptRepository,
        ITransactionRepository transactionRepository,
        IStorageService storageService,
        IOcrService ocrService)
    {
        _receiptRepository = receiptRepository;
        _transactionRepository = transactionRepository;
        _storageService = storageService;
        _ocrService = ocrService;
    }

    public async Task<ReceiptUploadResponseDto> UploadAsync(
        Guid userId,
        IFormFile file,
        Guid? transactionId = null)
    {
        // Validar arquivo
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        if (file.Length > 10 * 1024 * 1024) // 10MB
            throw new ArgumentException("File size cannot exceed 10MB");

        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Invalid file type. Only JPG, PNG, and PDF are allowed");

        // Validar transaction se fornecida
        if (transactionId.HasValue)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId.Value);
            if (transaction == null || transaction.UserId != userId)
                throw new ArgumentException("Invalid transaction");
        }

        // Upload do arquivo
        string fileUrl;
        using (var stream = file.OpenReadStream())
        {
            fileUrl = await _storageService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                "receipts");
        }

        // Criar entidade Receipt
        var receipt = new Receipt(
            userId,
            file.FileName,
            fileUrl,
            file.Length,
            file.ContentType
        );

        if (transactionId.HasValue)
        {
            receipt.LinkToTransaction(transactionId.Value);
        }

        await _receiptRepository.AddAsync(receipt);

        // Processar OCR de forma assíncrona (não espera completar)
        _ = ProcessOcrInBackground(receipt.Id, userId, fileUrl);

        return MapToUploadResponseDto(receipt, null);
    }

    public async Task<ReceiptDto> ProcessOcrAsync(Guid receiptId, Guid userId)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId);

        if (receipt == null || receipt.UserId != userId)
            throw new KeyNotFoundException("Receipt not found");

        OcrDataDto ocrData;
        try
        {
            ocrData = await _ocrService.ProcessImageAsync(receipt.FileUrl);

            var ocrDataJson = JsonSerializer.Serialize(ocrData);
            receipt.SetOcrData(ocrDataJson);
        }
        catch (Exception)
        {
            receipt.MarkOcrAsFailed();
            throw;
        }
        finally
        {
            await _receiptRepository.UpdateAsync(receipt);
        }

        return MapToDto(receipt, ocrData);
    }

    public async Task<CreateTransactionFromReceiptResponseDto> CreateTransactionFromReceiptAsync(
        Guid receiptId,
        Guid userId,
        CreateTransactionFromReceiptDto? dto = null)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId);

        if (receipt == null || receipt.UserId != userId)
            throw new KeyNotFoundException("Receipt not found");

        if (!receipt.OcrProcessed || string.IsNullOrWhiteSpace(receipt.OcrDataJson))
            throw new InvalidOperationException("OCR data not available. Process OCR first.");

        var ocrData = JsonSerializer.Deserialize<OcrDataDto>(receipt.OcrDataJson);
        if (ocrData == null)
            throw new InvalidOperationException("Invalid OCR data");

        // Criar transação com base nos dados do OCR
        var transaction = new Transaction(
            userId,
            TransactionType.Expense,
            ocrData.ExtractedValue ?? 0,
            dto?.Description ?? ocrData.MerchantName ?? "Despesa",
            dto?.Category ?? "Outros",
            ocrData.ExtractedDate ?? DateTime.UtcNow,
            dto?.ExpenseType ?? ExpenseType.Variaveis
        );

        await _transactionRepository.AddAsync(transaction);

        // Vincular receipt à transação
        receipt.LinkToTransaction(transaction.Id);
        transaction.AttachReceipt(receipt);

        await _receiptRepository.UpdateAsync(receipt);
        await _transactionRepository.UpdateAsync(transaction);

        return new CreateTransactionFromReceiptResponseDto
        {
            Transaction = MapTransactionToDto(transaction),
            Receipt = MapToDto(receipt, ocrData)
        };
    }

    private async Task ProcessOcrInBackground(Guid receiptId, Guid userId, string fileUrl)
    {
        try
        {
            await Task.Delay(1000); // Pequeno delay para simular processamento
            await ProcessOcrAsync(receiptId, userId);
        }
        catch
        {
            // Log error but don't throw
        }
    }

    private ReceiptUploadResponseDto MapToUploadResponseDto(Receipt receipt, OcrDataDto? ocrData)
    {
        return new ReceiptUploadResponseDto
        {
            Id = receipt.Id,
            TransactionId = receipt.TransactionId,
            FileName = receipt.FileName,
            FileUrl = receipt.FileUrl,
            FileSize = receipt.FileSize,
            MimeType = receipt.MimeType,
            OcrProcessed = receipt.OcrProcessed,
            OcrData = ocrData,
            CreatedAt = receipt.CreatedAt
        };
    }

    private ReceiptDto MapToDto(Receipt receipt, OcrDataDto? ocrData)
    {
        return new ReceiptDto
        {
            Id = receipt.Id,
            TransactionId = receipt.TransactionId,
            FileName = receipt.FileName,
            FileUrl = receipt.FileUrl,
            FileSize = receipt.FileSize,
            MimeType = receipt.MimeType,
            OcrProcessed = receipt.OcrProcessed,
            OcrData = ocrData,
            CreatedAt = receipt.CreatedAt
        };
    }

    private TransactionDto MapTransactionToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Type = transaction.Type,
            Value = transaction.Value,
            Description = transaction.Description,
            Category = transaction.Category,
            ExpenseType = transaction.ExpenseType,
            Date = transaction.Date,
            HasReceipt = transaction.HasReceipt,
            ReceiptUrl = transaction.ReceiptUrl,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}
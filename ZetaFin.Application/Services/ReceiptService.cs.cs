using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOcrService _ocrService;

    public ReceiptService(
        IReceiptRepository receiptRepository,
        ITransactionRepository transactionRepository,
        IFileStorageService fileStorageService,
        IOcrService ocrService)
    {
        _receiptRepository = receiptRepository;
        _transactionRepository = transactionRepository;
        _fileStorageService = fileStorageService;
        _ocrService = ocrService;
    }

    public async Task<ReceiptDto> UploadAsync(
        Guid userId,
        IFormFile file,
        Guid? transactionId = null)
    {
        // Validar se a transação existe (se foi fornecida)
        if (transactionId.HasValue)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId.Value);
            if (transaction == null)
                throw new Exception("Transação não encontrada");

            if (transaction.UserId != userId)
                throw new Exception("Você não tem permissão para acessar esta transação");
        }

        // Upload do arquivo
        var fileUrl = await _fileStorageService.UploadFileAsync(file, "receipts");

        // Criar o recibo
        var receipt = new Receipt(
            userId,
            file.FileName,
            fileUrl,
            file.Length,
            file.ContentType,
            transactionId
        );

        await _receiptRepository.AddAsync(receipt);

        // Processar OCR em background (ou síncronamente)
        try
        {
            var ocrData = await _ocrService.ProcessReceiptAsync(fileUrl);
            var ocrJson = JsonSerializer.Serialize(ocrData);
            receipt.MarkAsProcessed(ocrJson);
            await _receiptRepository.UpdateAsync(receipt);
        }
        catch (Exception ex)
        {
            // Log do erro, mas não falha o upload
            Console.WriteLine($"Erro ao processar OCR: {ex.Message}");
        }

        return MapToDto(receipt);
    }

    public async Task<ReceiptDto?> GetByIdAsync(Guid id)
    {
        var receipt = await _receiptRepository.GetByIdAsync(id);
        return receipt == null ? null : MapToDto(receipt);
    }

    public async Task<ReceiptDto> ProcessOcrAsync(Guid receiptId)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId);
        if (receipt == null)
            throw new Exception("Recibo não encontrado");

        var ocrData = await _ocrService.ProcessReceiptAsync(receipt.FileUrl);
        var ocrJson = JsonSerializer.Serialize(ocrData);

        receipt.UpdateOcrData(ocrJson);
        await _receiptRepository.UpdateAsync(receipt);

        return MapToDto(receipt);
    }

    public async Task<ReceiptWithTransactionDto> CreateTransactionFromOcrAsync(
        Guid receiptId,
        Guid userId,
        CreateTransactionFromOcrDto? dto = null)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId);
        if (receipt == null)
            throw new Exception("Recibo não encontrado");

        if (receipt.UserId != userId)
            throw new Exception("Você não tem permissão para acessar este recibo");

        if (!receipt.OcrProcessed || string.IsNullOrEmpty(receipt.OcrDataJson))
            throw new Exception("OCR ainda não foi processado para este recibo");

        if (receipt.TransactionId.HasValue)
            throw new Exception("Recibo já está vinculado a uma transação");

        // Deserializar dados do OCR
        var ocrData = JsonSerializer.Deserialize<OcrDataDto>(receipt.OcrDataJson);
        if (ocrData == null)
            throw new Exception("Dados do OCR inválidos");

        // Criar transação com dados do OCR (ou sobrescrever com dto)
        var transaction = new Transaction(
            userId,
            TransactionType.Expense, // Recibos geralmente são despesas
            ocrData.ExtractedValue ?? 0,
            dto?.Description ?? ocrData.MerchantName ?? "Despesa via recibo",
            dto?.Category ?? "Outros",
            ocrData.ExtractedDate ?? DateTime.UtcNow,
            dto?.ExpenseType ?? Domain.Entities.ExpenseType.Variaveis
        );

        await _transactionRepository.AddAsync(transaction);

        // Vincular recibo à transação
        receipt.LinkToTransaction(transaction.Id);
        transaction.AttachReceipt(receipt);

        await _receiptRepository.UpdateAsync(receipt);
        await _transactionRepository.UpdateAsync(transaction);

        return new ReceiptWithTransactionDto
        {
            Transaction = new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Type = transaction.Type.ToString().ToLower(),
                Value = transaction.Value,
                Description = transaction.Description,
                Category = transaction.Category,
                ExpenseType = transaction.ExpenseType?.ToString().ToLower(),
                Date = transaction.Date,
                HasReceipt = true,
                ReceiptUrl = receipt.FileUrl,
                CreatedAt = transaction.CreatedAt
            },
            Receipt = MapToDto(receipt)
        };
    }

    private ReceiptDto MapToDto(Receipt receipt)
    {
        OcrDataDto? ocrData = null;
        if (receipt.OcrProcessed && !string.IsNullOrEmpty(receipt.OcrDataJson))
        {
            try
            {
                ocrData = JsonSerializer.Deserialize<OcrDataDto>(receipt.OcrDataJson);
            }
            catch
            {
                // Se falhar ao deserializar, retorna null
            }
        }

        return new ReceiptDto
        {
            Id = receipt.Id,
            TransactionId = receipt.TransactionId,
            UserId = receipt.UserId,
            FileName = receipt.FileName,
            FileUrl = receipt.FileUrl,
            FileSize = receipt.FileSize,
            MimeType = receipt.MimeType,
            OcrProcessed = receipt.OcrProcessed,
            OcrData = ocrData,
            CreatedAt = receipt.CreatedAt
        };
    }
}
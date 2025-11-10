using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZetaFin.Application.DTOs;

namespace ZetaFin.Application.Interfaces;

// Interface do serviço de transações
public interface ITransactionService
{
    Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionDto dto);
    Task<TransactionDto?> GetByIdAsync(Guid id);
    Task<TransactionListDto> GetFilteredAsync(Guid userId, TransactionQueryDto query);
    Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<DetailedSummaryDto> GetSummaryAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
}

// Interface do serviço de recibos
public interface IReceiptService
{
    Task<ReceiptDto> UploadAsync(Guid userId, IFormFile file, Guid? transactionId = null);
    Task<ReceiptDto?> GetByIdAsync(Guid id);
    Task<ReceiptDto> ProcessOcrAsync(Guid receiptId);
    Task<ReceiptWithTransactionDto> CreateTransactionFromOcrAsync(Guid receiptId, Guid userId, CreateTransactionFromOcrDto? dto = null);
}

// Interface do serviço de armazenamento de arquivos
public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<byte[]> DownloadFileAsync(string fileUrl);
}

// Interface do serviço de OCR
public interface IOcrService
{
    Task<OcrDataDto> ProcessReceiptAsync(string fileUrl);
}
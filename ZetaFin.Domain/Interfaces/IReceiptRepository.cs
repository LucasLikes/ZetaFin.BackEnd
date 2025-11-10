using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Domain.Interfaces;

public interface IReceiptRepository
{
    Task<Receipt?> GetByIdAsync(Guid id);
    Task<IEnumerable<Receipt>> GetByUserIdAsync(Guid userId);
    Task<Receipt?> GetByTransactionIdAsync(Guid transactionId);
    Task<IEnumerable<Receipt>> GetUnprocessedAsync();
    Task AddAsync(Receipt receipt);
    Task UpdateAsync(Receipt receipt);
    Task DeleteAsync(Receipt receipt);
    Task<bool> ExistsAsync(Guid id);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Persistence.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly ApplicationDbContext _context;

    public ReceiptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Receipt?> GetByIdAsync(Guid id)
    {
        return await _context.Receipts
            .Include(r => r.Transaction)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Receipt>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Receipts
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Receipt?> GetByTransactionIdAsync(Guid transactionId)
    {
        return await _context.Receipts
            .FirstOrDefaultAsync(r => r.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Receipt>> GetUnprocessedAsync()
    {
        return await _context.Receipts
            .Where(r => !r.OcrProcessed)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Receipt receipt)
    {
        await _context.Receipts.AddAsync(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Receipt receipt)
    {
        _context.Receipts.Update(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Receipt receipt)
    {
        _context.Receipts.Remove(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Receipts.AnyAsync(r => r.Id == id);
    }
}
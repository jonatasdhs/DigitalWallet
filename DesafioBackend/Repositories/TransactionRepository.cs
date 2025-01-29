using DesafioBackend.DataContext;
using DesafioBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Repositories;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task CreateAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> ListAsync(int id, DateTime? startDate, DateTime? endDate)
    {
        startDate = (startDate ?? DateTime.MinValue).ToUniversalTime();
        endDate = (endDate ?? DateTime.MaxValue).ToUniversalTime();
        startDate.Value.ToUniversalTime();
        endDate.Value.ToUniversalTime();
        var result = await _context.Transactions
        .Where(t =>
            (t.ReceiverId == id || t.SenderId == id) &&
            t.CreatedAt >= startDate &&
            t.CreatedAt <= endDate)
        .ToListAsync();
        return result;
    }
}
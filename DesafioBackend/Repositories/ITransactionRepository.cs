using DesafioBackend.Models;

namespace DesafioBackend.Repositories;

public interface ITransactionRepository
{
    Task CreateAsync(Transaction transaction);
    Task<List<Transaction>> ListAsync(int id, DateTime? startDate, DateTime? endDate);
}
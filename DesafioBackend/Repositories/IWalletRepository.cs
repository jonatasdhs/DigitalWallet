using DesafioBackend.Models;

namespace DesafioBackend.Repositories;

public interface IWalletRepository
{
    Task CreateAsync(string userId);
    Task<Wallet?> ReadAsync(string userId);
    Task UpdateAsync(Wallet wallet);
    Task<User?> FindByIdentifierAsync(string identifier);
}
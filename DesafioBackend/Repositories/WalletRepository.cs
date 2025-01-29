using DesafioBackend.DataContext;
using DesafioBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Repositories;

public class WalletRepository(ApplicationDbContext context) : IWalletRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task CreateAsync(string userId)
    {
        var wallet = new Wallet
        {
            Balance = 0.0M,
            UserId = userId,
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task<Wallet?> ReadAsync(string userId)
    {
        var result = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.UserId == userId);
        return result;
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> FindByIdentifierAsync(string identifier)
    {
        Console.WriteLine(identifier.ToUpper());
        return await _context.Users
            .Where(u => u.NormalizedUserName == identifier.ToUpper() || u.NormalizedEmail == identifier.ToUpper() || u.Id == identifier)
            .FirstOrDefaultAsync();
    }
}
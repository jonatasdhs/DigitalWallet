using DesafioBackend.Models;
using DesafioBackend.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace DesafioBackend.DataContext;

public class DbSeeder(ApplicationDbContext context, UserManager<User> userManager)
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;

    public async Task SeedAsync()
    {
        if (!_context.Users.Any())
        {
            var users = new List<User>
            {
                new User { UserName = "lucas", Email = "lucas@example.com" },
                new User { UserName = "ana", Email = "ana@example.com" },
                new User { UserName = "pedro", Email = "pedro@example.com" },
                new User { UserName = "maria", Email = "maria@example.com" },
                new User { UserName = "joao", Email = "joao@example.com" },
                new User { UserName = "carla", Email = "carla@example.com" },
                new User { UserName = "tiago", Email = "tiago@example.com" },
                new User { UserName = "laura", Email = "laura@example.com" },
                new User { UserName = "luciana", Email = "luciana@example.com" },
                new User { UserName = "andre", Email = "andre@example.com" },
                new User { UserName = "user", Email = "user@mail.com"}
            };

            foreach (var user in users)
            {
                await _userManager.CreateAsync(user, "Senha123!");
            }

            var wallets = users.Select(user => new Wallet
            {
                User = user,
                Balance = new Random().Next(2000, 5000)
            }).ToList();

            await _context.Wallets.AddRangeAsync(wallets);

            await _context.SaveChangesAsync();

            var transactions = new List<Transaction>();

            for (int i = 0; i <= users.Count; i++)
            {
                for (int j = 0; j <= users.Count; j++)
                {

                    var senderWallet = wallets[i % users.Count];
                    var receiverWallet = wallets[j % users.Count];

                    var transaction = new Transaction
                    {
                        Amount = new Random().Next(50, 500),
                        SenderId = senderWallet.Id,
                        ReceiverId = receiverWallet.Id,
                        TransactionTypes = senderWallet.Id == receiverWallet.Id ? TransactionTypes.Deposit : TransactionTypes.Transfer,
                        Description = $"Transação de {senderWallet.User.UserName} para {receiverWallet.User.UserName}",
                        CreatedAt = DateTime.UtcNow
                    };

                    transactions.Add(transaction);
                }
            }

            await _context.Transactions.AddRangeAsync(transactions);

            await _context.SaveChangesAsync();
        }
    }
}
using DesafioBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DesafioBackend.DataContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public new DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasOne(user => user.Wallet)
                .WithOne(wallet => wallet.User)
                .HasForeignKey<Wallet>(wallet => wallet.UserId);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Sender)
                .WithMany(wallet => wallet.SentTransactions)
                .HasForeignKey(transaction => transaction.SenderId);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Receiver)
                .WithMany(wallet => wallet.ReceivedTransactions)
                .HasForeignKey(transaction => transaction.ReceiverId);
        }
    }
}
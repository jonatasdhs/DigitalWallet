using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioBackend.Models;

public class Wallet
{
    [Key]
    public int Id { get; set; }
    [Required]
    public decimal Balance { get; set; }
    [Required]
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    [JsonIgnore]
    public ICollection<Transaction> SentTransactions { get; set; } = null!;
    [JsonIgnore]
    public ICollection<Transaction> ReceivedTransactions { get; set; } = null!;
}
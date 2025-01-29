using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DesafioBackend.Models.Enum;

namespace DesafioBackend.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    [Required]
    public decimal Amount { get; set; }
    [Required]
    public int SenderId { get; set; }
    [Required]
    public int ReceiverId { get; set; }
    [MaxLength(255)]
    public string? Description { get; set; }
    public TransactionTypes TransactionTypes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public Wallet Sender { get; set; } = null!;
    [JsonIgnore]
    public Wallet Receiver { get; set; } = null!;
}
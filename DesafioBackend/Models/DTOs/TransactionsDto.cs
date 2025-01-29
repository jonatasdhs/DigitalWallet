namespace DesafioBackend.Models.DTOs;

public class TransactionsDto
{
    public List<Transaction> Received { get; set; } = null!;
    public List<Transaction> Sent { get; set; } = null!;
    public List<Transaction> Deposit { get; set; } = null!;
    public List<Transaction> Withdraw { get; set; } = null!;
}
namespace DesafioBackend.Models.DTOs;

public class WalletReturnDto(decimal Value)
{
    public decimal Balance { get; set; } = Value;
}
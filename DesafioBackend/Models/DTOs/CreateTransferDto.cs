namespace DesafioBackend.Models.DTOs;
public class CreateTransferDto
{
    public string Identifier { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

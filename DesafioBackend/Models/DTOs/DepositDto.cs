using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Models.DTOs;
public class DepositDto
{
    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Models.DTOs;
public class LoginDto
{
    [Required(ErrorMessage = "Campo 'Email' é obrigatório!")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Campo 'Password' é obrigatório!")]
    public required string Password { get; set; }
}
namespace DesafioBackend.Models.DTOs;

public class UserDTO
{
    public string? Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
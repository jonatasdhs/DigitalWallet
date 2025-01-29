namespace DesafioBackend.Models.DTOs;

public class TokenDto(string token) 
{
    public string Token { get; set; } = token;
}
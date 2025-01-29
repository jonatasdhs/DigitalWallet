using DesafioBackend.Models;

namespace DesafioBackend.Services.TokenService;

public interface ITokenService
{
    string GenerateJwtToken(User user);
}
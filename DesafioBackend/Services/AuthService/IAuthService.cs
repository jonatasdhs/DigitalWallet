using DesafioBackend.Models.DTOs;
using DesafioBackend.Utils;

namespace DesafioBackend.Services.AuthService;

public interface IAuthService
{
    Task<Result<TokenDto>> LoginAsync(LoginDto loginDto);
}
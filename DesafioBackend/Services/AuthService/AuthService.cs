using DesafioBackend.Exceptions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Repositories;
using DesafioBackend.Services.TokenService;
using DesafioBackend.Utils;

namespace DesafioBackend.Services.AuthService;

public class AuthService(IUserRepository userRepository, ITokenService tokenService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;
    public async Task<Result<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.FindByEmailAsync(loginDto.Email);
        if(user == null)
        {
            return Result.Failure<TokenDto>(UserErrors.EmailNotFound);
        }
        var result = await _userRepository.SignInAsync(user, loginDto.Password);
        if(result.Succeeded == false)
        {
            return Result.Failure<TokenDto>(UserErrors.UserNotFound);
        }

        var token = _tokenService.GenerateJwtToken(user);
        return Result.Success(new TokenDto(token));
    }
}
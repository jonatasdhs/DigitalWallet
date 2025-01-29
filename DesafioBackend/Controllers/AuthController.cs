using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IResult> LoginAsync([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result.IsFailure)
        {
            return Results.Unauthorized();
        }
    
        return Results.Ok(result);
    }
}
using DesafioBackend.Extensions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Controllers;

[Route("api/users")]
[ApiController]

public class UserController(IUserService userService): ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("")]
    public async Task<IResult> RegisterUser([FromBody] CreateUserDTO createUserDTO)
    {
        if(!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        var result = await _userService.CreateUser(createUserDTO);

        if(result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return Results.Created($"/users", result);
    }
}
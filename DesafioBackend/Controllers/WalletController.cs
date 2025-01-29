using System.Security.Claims;
using DesafioBackend.Extensions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.WalletService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.Controllers;

[Route("api/wallet")]
[ApiController]
public class WalletController(IWalletService walletService) : ControllerBase
{
    private readonly IWalletService _walletService = walletService;

    [Authorize]
    [HttpGet()]
    public async Task<IResult> Balance()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }
        var result = await _walletService.GetBalanceAsync(userId);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }
        return Results.Ok(result);
    }
    [Authorize]
    [HttpPost()]
    public async Task<IResult> Deposit([FromBody] DepositDto depositDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }
        var result = await _walletService.DepositAsync(userId, depositDto);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }
        return Results.Created("/api/wallet", result);
    }

    [Authorize]
    [HttpPost("transfer")]
    public async Task<IResult> Transfer([FromBody] CreateTransferDto transferDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }
        var result = await _walletService.TransferAsync(transferDto, userId);

        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return Results.Ok(result);
    }

    [Authorize]
    [HttpGet("transfer")]
    public async Task<IResult> GetTransferAsync([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }
        var result = await _walletService.GetTransactionsAsync(userId, startDate, endDate);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }
        return Results.Ok(result);
    }
}
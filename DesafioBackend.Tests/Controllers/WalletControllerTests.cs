using DesafioBackend.Controllers;
using DesafioBackend.Exceptions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.WalletService;
using DesafioBackend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace DesafioBackend.Tests.Controllers;

public class WalletControllerTests
{
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _controller = new WalletController(_walletServiceMock.Object);
    }

    private void SetUpUser(string userId)
    {
        var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(userClaims, "test");
        var userPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userPrincipal }
        };
    }

    [Fact]
    public async Task WalletController_ShouldReturnOk_WhenBalanceIsRetrievedSuccessfully()
    {
        // Arrange
        var userId = "user123";
        var mockBalance = new decimal(100.00);
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.GetBalanceAsync(userId))
                          .ReturnsAsync(Result.Success(new WalletReturnDto(mockBalance)));

        // Act
        var result = await _controller.Balance();

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Result<WalletReturnDto>>>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(mockBalance, okResult.Value?.Value.Balance);
    }

    [Fact]
    public async Task WalletController_ShouldReturnBadRequest_WhenBalanceRetrievalFails()
    {
        // Arrange
        var userId = "user123";
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.GetBalanceAsync(userId))
                          .ReturnsAsync(Result.Failure<WalletReturnDto>(WalletErrors.NotFound));

        // Act
        var result = await _controller.Balance();

        // Assert
        var problemDetailsResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(404, problemDetailsResult.StatusCode);
    }

    [Fact]
    public async Task WalletController_ShouldReturnCreated_WhenDepositIsSuccessful()
    {
        // Arrange
        var userId = "user123";
        var depositDto = new DepositDto { Amount = 50 };
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.DepositAsync(userId, depositDto))
                          .ReturnsAsync(Result.Success("Deposit successful"));

        // Act
        var result = await _controller.Deposit(depositDto);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<Result<string>>>(result);
        Assert.Equal("/api/wallet", createdResult.Location);
        Assert.Equal("Deposit successful", createdResult.Value?.Value);
    }

    [Fact]
    public async Task WalletController_ShouldReturnBadRequest_WhenDepositFails()
    {
        // Arrange
        var userId = "user123";
        var depositDto = new DepositDto { Amount = 50 };
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.DepositAsync(userId, depositDto))
                          .ReturnsAsync(Result.Failure<string>(WalletErrors.BadRequest));

        // Act
        var result = await _controller.Deposit(depositDto);

        // Assert
        var problemDetailsResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(400, problemDetailsResult.StatusCode);
    }

    [Fact]
    public async Task WalletController_ShouldReturnOk_WhenTransferIsSuccessful()
    {
        // Arrange
        var userId = "user123";
        var transferDto = new CreateTransferDto { Amount = 20, Identifier = "user456" };
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.TransferAsync(transferDto, userId))
                          .ReturnsAsync(Result.Success("Transfer successful"));

        // Act
        var result = await _controller.Transfer(transferDto);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Result<string>>>(result);
        Assert.Equal("Transfer successful", okResult.Value?.Value);
    }

    [Fact]
    public async Task WalletController_ShouldReturnBadRequest_WhenTransferFails()
    {
        // Arrange
        var userId = "user123";
        var transferDto = new CreateTransferDto { Amount = 20, Identifier = "user456" };
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.TransferAsync(transferDto, userId))
                          .ReturnsAsync(Result.Failure<string>(TransferErrors.BadRequest));

        // Act
        var result = await _controller.Transfer(transferDto);

        // Assert
        var problemDetailsResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(400, problemDetailsResult.StatusCode);
    }

    [Fact]
    public async Task WalletController_ShouldReturnOk_WhenGetTransferTransactionsIsSuccessful()
    {
        // Arrange
        var userId = "user123";
        SetUpUser(userId);
        var transactionDto = new TransactionsDto
        {

        };

        _walletServiceMock.Setup(service => service.GetTransactionsAsync(userId, null, null))
                          .ReturnsAsync(Result.Success(transactionDto));

        // Act
        var result = await _controller.GetTransferAsync(null, null);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Result<TransactionsDto>>>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task WalletController_ShouldReturnBadRequest_WhenGetTransferTransactionsFails()
    {
        // Arrange
        var userId = "user123";
        SetUpUser(userId);

        _walletServiceMock.Setup(service => service.GetTransactionsAsync(userId, null, null))
                          .ReturnsAsync(Result.Failure<TransactionsDto>(WalletErrors.NotFound));

        // Act
        var result = await _controller.GetTransferAsync(null, null);

        // Assert
        var problemDetailsResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal(404, problemDetailsResult.StatusCode);
    }
}


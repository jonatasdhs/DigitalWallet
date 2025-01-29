using DesafioBackend.Controllers;
using DesafioBackend.Exceptions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.AuthService;
using DesafioBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly Mock<IAuthService> _authServiceMock;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@example.com",
            Password = "Password123"
        };

        var tokenDto = new TokenDto("fake-jwt-token");

        _authServiceMock.Setup(service => service.LoginAsync(loginDto))
            .ReturnsAsync(Result.Success(tokenDto));

        // Act
        var result = await _authController.LoginAsync(loginDto);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Result<TokenDto>>>(result); // Verifica o tipo Result<TokenDto>
        Assert.Equal(200, okResult.StatusCode);  // Verifica o código de status
        Assert.IsType<TokenDto>(okResult.Value?.Value); // Verifica o tipo TokenDto dentro de Result
        Assert.Equal("fake-jwt-token", okResult.Value.Value.Token);
    }
    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenLoginFails()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@example.com",
            Password = "InvalidPassword"
        };

        _authServiceMock.Setup(service => service.LoginAsync(loginDto))
            .ReturnsAsync(Result.Failure<TokenDto>(UserErrors.UserNotFound));

        // Act
        var result = await _authController.LoginAsync(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
}

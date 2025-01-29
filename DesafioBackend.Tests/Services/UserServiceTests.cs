using DesafioBackend.Repositories;
using Moq;
using DesafioBackend.Services.UserService;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Models;
using DesafioBackend.Exceptions;
namespace DesafioBackend.Tests.Service;

public class UserServiceTests
{
    private readonly UserService userService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        userService = new UserService(_userRepositoryMock.Object, _walletRepositoryMock.Object);
    }
    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Email = "newuser@example.com",
            UserName = "newUser",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(createUserDto.Email))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(repo => repo.UserNameExistsAsync(createUserDto.UserName))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(true);

        _walletRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await userService.CreateUser(createUserDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createUserDto.Email, result.Value.Email);
        Assert.Equal(createUserDto.UserName, result.Value.UserName);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Email = "existinguser@example.com",
            UserName = "existingUser",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(createUserDto.Email))
            .ReturnsAsync(true);

        // Act
        var result = await userService.CreateUser(createUserDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.AlreadyExists, result.Error);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUserNameAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Email = "newuser@example.com",
            UserName = "existingUser",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(createUserDto.Email))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(repo => repo.UserNameExistsAsync(createUserDto.UserName))
            .ReturnsAsync(true);

        // Act
        var result = await userService.CreateUser(createUserDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.AlreadyExists, result.Error);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUserCreationFails()
    {
        // Arrange
        var createUserDto = new CreateUserDTO
        {
            Email = "newuser@example.com",
            UserName = "newUser",
            Password = "Password123!"
        };

        _userRepositoryMock.Setup(repo => repo.UserExistsAsync(createUserDto.Email))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(repo => repo.UserNameExistsAsync(createUserDto.UserName))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(false);

        // Act
        var result = await userService.CreateUser(createUserDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.BadRequest, result.Error);
    }
}
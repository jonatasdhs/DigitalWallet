using System.Threading.Tasks;
using DesafioBackend.Exceptions;
using DesafioBackend.Models;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Repositories;
using DesafioBackend.Services.AuthService;
using DesafioBackend.Services.TokenService;
using DesafioBackend.Utils;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DesafioBackend.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _authService = new AuthService(_userRepositoryMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenEmailNotFound()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null); 

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UserErrors.EmailNotFound, result.Error);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "existinguser@example.com",
                Password = "WrongPassword123!"
            };

            var user = new User
            {
                Email = loginDto.Email
            };

            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(repo => repo.SignInAsync(user, loginDto.Password))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(UserErrors.UserNotFound, result.Error);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "validuser@example.com",
                Password = "ValidPassword123!"
            };

            var user = new User
            {
                Email = loginDto.Email
            };

            var token = "valid-jwt-token";

            _userRepositoryMock.Setup(repo => repo.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(repo => repo.SignInAsync(user, loginDto.Password))
                .ReturnsAsync(SignInResult.Success);

            _tokenServiceMock.Setup(service => service.GenerateJwtToken(user))
                .Returns(token);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(token, result.Value.Token);
        }
    }
}

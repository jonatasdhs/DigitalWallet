using System.Threading.Tasks;
using DesafioBackend.Controllers;
using DesafioBackend.Exceptions;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Services.UserService;
using DesafioBackend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DesafioBackend.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnCreated_WhenUserIsSuccessfullyCreated()
        {
            // Arrange
            var createUserDto = new CreateUserDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                UserName = "testUser"
            };

            var userDto = new UserDTO
            {
                Id = "user123",
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
                CreatedAt = System.DateTime.UtcNow
            };

            _userServiceMock
                .Setup(service => service.CreateUser(createUserDto))
                .ReturnsAsync(Result.Success(userDto));

            // Act
            var result = await _userController.RegisterUser(createUserDto);

            // Assert
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<Result<UserDTO>>>(result);
            Assert.Equal($"/users", createdResult.Location);

            var responseValue = Assert.IsType<Result<UserDTO>>(createdResult.Value);
            Assert.True(responseValue.IsSuccess);
            Assert.Equal(userDto.Email, responseValue.Value.Email);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _userController.ModelState.AddModelError("Email", "The Email field is required.");

            var createUserDto = new CreateUserDTO
            {
                Password = "Password123!",
                UserName = "testUser"
            };

            // Act
            var result = await _userController.RegisterUser(createUserDto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>>(result);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnProblemDetails_WhenServiceFails()
        {
            // Arrange
            var createUserDto = new CreateUserDTO
            {
                Email = "test@example.com",
                Password = "Password123!",
                UserName = "testUser"
            };

            _userServiceMock
                .Setup(service => service.CreateUser(It.IsAny<CreateUserDTO>()))
                .ReturnsAsync(Result.Failure<UserDTO>(UserErrors.AlreadyExists));

            // Act
            var result = await _userController.RegisterUser(createUserDto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using DesafioBackend.Exceptions;
using DesafioBackend.Models;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Repositories;
using DesafioBackend.Utils;
using Microsoft.AspNetCore.Identity;

namespace DesafioBackend.Services.UserService;

public class UserService(IUserRepository userRepository, IWalletRepository walletRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IWalletRepository _walletRepository = walletRepository;
    public async Task<Result<UserDTO>> CreateUser(CreateUserDTO createUserDTO)
    {
        var userExists = await _userRepository.UserExistsAsync(createUserDTO.Email);
        if (userExists)
        {
            return Result.Failure<UserDTO>(UserErrors.AlreadyExists);
        }
        var userNameExists = await _userRepository.UserNameExistsAsync(createUserDTO.UserName);
        if (userNameExists)
        {
            return Result.Failure<UserDTO>(UserErrors.AlreadyExists);
        }
        var user = new User
        {
            Email = createUserDTO.Email,
            CreatedAt = DateTime.UtcNow,
            UserName = createUserDTO.UserName
        };
        var result = await _userRepository.CreateAsync(user, createUserDTO.Password);
        if (!result)
        {
            return Result.Failure<UserDTO>(UserErrors.BadRequest);
        }

        await _walletRepository.CreateAsync(user.Id);

        var userDto = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt
        };

        return Result.Success(userDto);
    }
}
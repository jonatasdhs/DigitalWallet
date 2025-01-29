using DesafioBackend.Models.DTOs;
using DesafioBackend.Utils;

namespace DesafioBackend.Services.UserService;

public interface IUserService
{
    Task<Result<UserDTO>> CreateUser(CreateUserDTO createUserDTO);
}
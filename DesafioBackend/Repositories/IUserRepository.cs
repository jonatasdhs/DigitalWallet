using DesafioBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace DesafioBackend.Repositories;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user, string password);
    Task<User?> FindByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string username);
    Task<SignInResult> SignInAsync(User user, string password);
}
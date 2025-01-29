using DesafioBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Repositories;

public class UserRepository(UserManager<User> userManager, SignInManager<User> signInManager) : IUserRepository
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    public async Task<bool> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }
    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<bool> UserNameExistsAsync(string userName)
    {
        var user = await _userManager.Users.AnyAsync(user => user.UserName == userName);
        return user;
    }
    public async Task<SignInResult> SignInAsync(User user, string password)
    {
        return await _signInManager.CheckPasswordSignInAsync(user, password, false);
    }
}

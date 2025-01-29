using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DesafioBackend.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Wallet Wallet { get; set; } = null!;
}
using DesafioBackend.Models;

namespace DesafioBackend.Exceptions;
public static class WalletErrors
{
    public static readonly Error AlreadyExists = Error.Conflict("Wallet.AlreadyExists", "Email already exists!");
    public static readonly Error NotFound = Error.NotFound("Wallet.NotFound", "User does not exists");
    public static readonly Error BadRequest = Error.Validation("Wallet.BadRequest", "Bad request");
    public static readonly Error WalletNotFound = Error.Validation("Wallet.NotFound", "User Id not found");
}
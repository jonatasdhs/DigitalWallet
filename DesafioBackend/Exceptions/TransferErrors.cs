using DesafioBackend.Models;

namespace DesafioBackend.Exceptions;
public static class TransferErrors
{
    public static readonly Error AlreadyExists = Error.Conflict("Wallet.AlreadyExists", "Email already exists!");
    public static readonly Error NotFound = Error.NotFound("Transaction.NotFound", "Wallet does not exists");
    public static readonly Error BadRequest = Error.Validation("Transaction.BadRequest", "Insuficient funds");
    public static readonly Error WalletNotFound = Error.Validation("Wallet.NotFound", "User Id not found");
}
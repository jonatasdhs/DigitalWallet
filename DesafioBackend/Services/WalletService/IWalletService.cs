using DesafioBackend.Models.DTOs;
using DesafioBackend.Utils;

namespace DesafioBackend.Services.WalletService;

public interface IWalletService
{
    Task<Result<WalletReturnDto>> GetBalanceAsync(string userId);
    Task<Result<string>> DepositAsync(string userId, DepositDto depositDto);
    Task<Result<string>> TransferAsync(CreateTransferDto transferDto, string userId);
    Task<Result<TransactionsDto>> GetTransactionsAsync(string userId, DateTime? startDate, DateTime? endDate);
}
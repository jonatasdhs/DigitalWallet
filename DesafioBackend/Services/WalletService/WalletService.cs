
using System.Text.Json;
using DesafioBackend.Exceptions;
using DesafioBackend.Models;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Models.Enum;
using DesafioBackend.Repositories;
using DesafioBackend.Utils;

namespace DesafioBackend.Services.WalletService;

public class WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository) : IWalletService
{
    private readonly IWalletRepository _walletRepository = walletRepository;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    public async Task<Result<WalletReturnDto>> GetBalanceAsync(string userId)
    {
        var wallet = await _walletRepository.ReadAsync(userId);
        if (wallet == null)
        {
            return Result.Failure<WalletReturnDto>(WalletErrors.NotFound);
        }

        return Result.Success(new WalletReturnDto(wallet.Balance));
    }

    public async Task<Result<string>> DepositAsync(string userId, DepositDto depositDto)
    {
        var wallet = await _walletRepository.ReadAsync(userId);
        if (wallet == null)
        {
            return Result.Failure<string>(WalletErrors.NotFound);
        }

        if (depositDto.Amount <= 0)
        {
            return Result.Failure<string>(WalletErrors.BadRequest);
        }

        var transaction = new Transaction
        {
            Amount = depositDto.Amount,
            CreatedAt = DateTime.UtcNow,
            ReceiverId = wallet.Id,
            SenderId = wallet.Id,
            TransactionTypes = TransactionTypes.Deposit
        };

        wallet.Balance += depositDto.Amount;

        await _transactionRepository.CreateAsync(transaction);
        await _walletRepository.UpdateAsync(wallet);
        return Result.Success("Saldo depositado com sucesso");
    }

    public async Task<Result<string>> TransferAsync(CreateTransferDto transferDto, string userId)
    {
        var senderWallet = await _walletRepository.ReadAsync(userId);

        var receiver = await _walletRepository.FindByIdentifierAsync(transferDto.Identifier);
        if (senderWallet is null || receiver is null)
        {
            return Result.Failure<string>(TransferErrors.NotFound);
        }

        var receiverWallet = await _walletRepository.ReadAsync(receiver.Id);

        if (receiverWallet is null)
        {
            return Result.Failure<string>(TransferErrors.NotFound);
        }

        if (senderWallet.Balance < transferDto.Amount)
        {
            return Result.Failure<string>(TransferErrors.BadRequest);
        }

        var transaction = new Transaction
        {
            Amount = transferDto.Amount,
            CreatedAt = DateTime.UtcNow,
            ReceiverId = receiverWallet.Id,
            SenderId = senderWallet.Id,
            Description = transferDto.Description,
            TransactionTypes = TransactionTypes.Transfer
        };

        senderWallet.Balance -= transferDto.Amount;
        receiverWallet.Balance += transferDto.Amount;

        await _transactionRepository.CreateAsync(transaction);
        await _walletRepository.UpdateAsync(senderWallet);
        await _walletRepository.UpdateAsync(receiverWallet);

        return Result.Success("Transaction completed");
    }

    public async Task<Result<TransactionsDto>> GetTransactionsAsync(string userId, DateTime? startDate, DateTime? endDate)
    {
        var wallet = await _walletRepository.ReadAsync(userId);
        if (wallet is null)
        {
            return Result.Failure<TransactionsDto>(WalletErrors.NotFound);
        }
        var transactions = await _transactionRepository.ListAsync(wallet.Id, startDate, endDate);
        
        var transactionDto = new TransactionsDto
        {
            Deposit = transactions.FindAll(t => t.TransactionTypes == TransactionTypes.Deposit),
            Withdraw = transactions.FindAll(t => t.TransactionTypes == TransactionTypes.Withdraw),
            Sent = transactions.FindAll(t => t.SenderId == wallet.Id && t.TransactionTypes == TransactionTypes.Transfer),
            Received = transactions.FindAll(t => t.ReceiverId == wallet.Id && t.TransactionTypes == TransactionTypes.Transfer)
        };
        return Result.Success(transactionDto);
    }
}
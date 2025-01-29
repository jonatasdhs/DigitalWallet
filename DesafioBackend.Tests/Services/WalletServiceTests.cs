using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioBackend.Exceptions;
using DesafioBackend.Models;
using DesafioBackend.Models.DTOs;
using DesafioBackend.Models.Enum;
using DesafioBackend.Repositories;
using DesafioBackend.Services.WalletService;
using DesafioBackend.Utils;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace DesafioBackend.Tests.Services
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly WalletService _walletService;
        private readonly ITestOutputHelper _output;

        public WalletServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _walletService = new WalletService(_walletRepositoryMock.Object, _transactionRepositoryMock.Object);
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldReturnBalance_WhenWalletExists()
        {
            // Arrange
            var userId = "user123";
            var wallet = new Wallet { Id = 1, Balance = 500 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetBalanceAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(wallet.Balance, result.Value.Balance);
        }

        [Fact]
        public async Task GetBalanceAsync_ShouldReturnFailure_WhenWalletDoesNotExist()
        {
            // Arrange
            var userId = "user123";

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync((Wallet?)null);

            // Act
            var result = await _walletService.GetBalanceAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(WalletErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task DepositAsync_ShouldIncreaseBalance_WhenValidDeposit()
        {
            // Arrange
            var userId = "user123";
            var wallet = new Wallet { Id = 1, Balance = 500 };
            var depositDto = new DepositDto { Amount = 200 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.DepositAsync(userId, depositDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Saldo depositado com sucesso", result.Value);
            Assert.Equal(700, wallet.Balance);

            _transactionRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Transaction>()), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(wallet), Times.Once);
        }

        [Fact]
        public async Task DepositAsync_ShouldReturnFailure_WhenWalletDoesNotExist()
        {
            // Arrange
            var userId = "user123";
            var depositDto = new DepositDto { Amount = 200 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync((Wallet?)null);

            // Act
            var result = await _walletService.DepositAsync(userId, depositDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(WalletErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task DepositAsync_ShouldReturnFailure_WhenAmountIsInvalid()
        {
            // Arrange
            var userId = "user123";
            var wallet = new Wallet { Id = 1, Balance = 500 };
            var depositDto = new DepositDto { Amount = -100 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync(wallet);

            // Act
            var result = await _walletService.DepositAsync(userId, depositDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(WalletErrors.BadRequest, result.Error);
        }

        [Fact]
        public async Task TransferAsync_ShouldTransferAmount_WhenValid()
        {
            // Arrange
            var userSender = new User { Id = "user1", Email="example@example.com" };
            var userReceiver = new User { Id = "user2", Email="example@example.com" };
            var senderWallet = new Wallet { Id = 1, Balance = 500, UserId = userSender.Id };
            var receiverWallet = new Wallet { Id = 2, Balance = 100, UserId = userReceiver.Id };
            var receiver = new Wallet { Id = 3 };
            var transferDto = new CreateTransferDto { Identifier = "user2", Amount = 50 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userSender.Id))
                .ReturnsAsync(senderWallet);

            _walletRepositoryMock.Setup(repo => repo.FindByIdentifierAsync(transferDto.Identifier))
                .ReturnsAsync(userReceiver);

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userReceiver.Id))
                .ReturnsAsync(receiverWallet);

            // Act
            var result = await _walletService.TransferAsync(transferDto, userSender.Id);
            _output.WriteLine(JsonSerializer.Serialize(result.Error));
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Transaction completed", result.Value);
            Assert.Equal(450, senderWallet.Balance);
            Assert.Equal(150, receiverWallet.Balance);

            _transactionRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Transaction>()), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(senderWallet), Times.Once);
            _walletRepositoryMock.Verify(repo => repo.UpdateAsync(receiverWallet), Times.Once);
        }

        [Fact]
        public async Task TransferAsync_ShouldReturnFailure_WhenSenderWalletNotFound()
        {
            // Arrange
            var userId = "user123";
            var transferDto = new CreateTransferDto { Identifier = "receiverIdentifier", Amount = 200 };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync((Wallet?)null);

            // Act
            var result = await _walletService.TransferAsync(transferDto, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(TransferErrors.NotFound, result.Error);
        }

        [Fact]
        public async Task GetTransactionsAsync_ShouldReturnTransactions_WhenWalletExists()
        {
            // Arrange
            var userId = "user123";
            var wallet = new Wallet { Id = 1 };
            var transactions = new List<Transaction>
            {
                new Transaction { Amount = 100, TransactionTypes = TransactionTypes.Deposit },
                new Transaction { Amount = 50, TransactionTypes = TransactionTypes.Withdraw },
                new Transaction { Amount = 200, TransactionTypes = TransactionTypes.Transfer, SenderId = 1 },
                new Transaction { Amount = 150, TransactionTypes = TransactionTypes.Transfer, ReceiverId = 1 }
            };

            _walletRepositoryMock.Setup(repo => repo.ReadAsync(userId))
                .ReturnsAsync(wallet);

            _transactionRepositoryMock.Setup(repo => repo.ListAsync(wallet.Id, null, null))
                .ReturnsAsync(transactions);

            // Act
            var result = await _walletService.GetTransactionsAsync(userId, null, null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value.Deposit);
            Assert.Single(result.Value.Withdraw);
            Assert.Single(result.Value.Sent);
            Assert.Single(result.Value.Received);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transfer;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Application.Services;
using DesyBank.Domain.Models;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DesyBank.Tests.Application.Services
{
    public class TransferServiceTests
    {
        // Mock
        private readonly ITransferRepository _transferRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDbRepository _dbRepository;
        private readonly IValidator<TransferRequest> _validator;
        private readonly TransferService _sut;

        public TransferServiceTests()
        {
            _transferRepository = Substitute.For<ITransferRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _transactionRepository = Substitute.For<ITransactionRepository>();
            _dbRepository = Substitute.For<IDbRepository>();
            _validator = Substitute.For<IValidator<TransferRequest>>();
            _sut = new TransferService(_transferRepository, _accountRepository, _validator, _transactionRepository, _dbRepository);
        }

        // Tests
        [Fact]
        [Trait("Services", "TransferService")]
        public async Task CreateTransfer_WithValidData_ReturnsTransferResponse()
        {
            // Arrange
            var fromUserId = Guid.NewGuid();
            var toUserId = Guid.NewGuid();
            var amount = 100m;

            var fromAccount = new Account(fromUserId, "12345-6");
            var toAccount = new Account(toUserId, "67890-1");
            fromAccount.Deposit(500m); // saldo suficiente

            var request = new TransferRequest(fromUserId, toAccount.AccountNumber, amount);

            _validator.ValidateAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(fromUserId, Arg.Any<CancellationToken>())
                .Returns(fromAccount);

            _accountRepository.GetAcountByNumberAsync(toAccount.AccountNumber, Arg.Any<CancellationToken>())
                .Returns(toAccount);

            // Act
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.FromAccountId.Should().Be(fromAccount.Id);
            response.ToAccountId.Should().Be(toAccount.Id);
            response.Amount.Should().Be(amount);
            response.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));

            // Verifica que salvou
            await _transferRepository.Received(1).AddAsync(Arg.Any<Transfer>(), Arg.Any<CancellationToken>());
            await _transactionRepository.Received(2).AddAsync(Arg.Any<Transaction>(), Arg.Any<CancellationToken>());
            await _dbRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "TransferService")]
        public async Task CreateTransfer_WhenFromAccountNotFound_ReturnsAccountNotFoundError()
        {
            // Arrange
            var fromUserId = Guid.NewGuid();
            var toAccountNumber = "67890-1";
            var amount = 100m;

            var request = new TransferRequest(fromUserId, toAccountNumber, amount);

            _validator.ValidateAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(fromUserId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // Act
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<AccountNotFoundError>();
        }

        [Fact]
        [Trait("Services", "TransferService")]
        public async Task CreateTransfer_WhenToAccountNotFound_ReturnsAccountNotFoundError()
        {
            // Arrange
            var fromUserId = Guid.NewGuid();
            var toAccountNumber = "67890-1";
            var amount = 100m;

            var fromAccount = new Account(fromUserId, "12345-6");

            var request = new TransferRequest(fromUserId, toAccountNumber, amount);

            _validator.ValidateAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(fromUserId, Arg.Any<CancellationToken>())
                .Returns(fromAccount);

            _accountRepository.GetAcountByNumberAsync(toAccountNumber, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // Act
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<AccountNotFoundError>();
        }

        [Fact]
        [Trait("Services", "TransferService")]
        public async Task CreateTransfer_WhenTransferToSameAccount_ReturnsCantTransferForOwnAccountError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var account = new Account(userId, "12345-6");
            account.Deposit(500m);

            var request = new TransferRequest(userId, account.AccountNumber, 100m);

            _validator.ValidateAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(account);

            _accountRepository.GetAcountByNumberAsync(account.AccountNumber, Arg.Any<CancellationToken>())
                .Returns(account);

            // Act
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<CantTransferForOwnAccountError>();
        }

        [Fact]
        [Trait("Services", "TransferService")]
        public async Task CreateTransfer_WithInsufficientBalance_ReturnsInsufficientBalanceError()
        {
            // Arrange
            var fromUserId = Guid.NewGuid();
            var toUserId = Guid.NewGuid();
            var amount = 500m;

            var fromAccount = new Account(fromUserId, "12345-6");
            var toAccount = new Account(toUserId, "67890-1");
            fromAccount.Deposit(100m); // saldo insuficiente

            var request = new TransferRequest(fromUserId, toAccount.AccountNumber, amount);

            _validator.ValidateAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(fromUserId, Arg.Any<CancellationToken>())
                .Returns(fromAccount);

            _accountRepository.GetAcountByNumberAsync(toAccount.AccountNumber, Arg.Any<CancellationToken>())
                .Returns(toAccount);

            // Act
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<InsufficientBalanceError>();
        }
    }
}
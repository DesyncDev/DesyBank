using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Application.Services;
using DesyBank.Domain.Enums;
using DesyBank.Domain.Models;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ReturnsExtensions;
using OneOf.Types;

namespace DesyBank.Tests.Application.Services
{
    public class TransactionServiceTests
    {
        // Faker
        private readonly Faker _faker = new("pt_BR");

        // Mock
        private readonly IValidator<TransactionRequest> _validator;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        // Service
        private readonly TransactionService _sut;

        public TransactionServiceTests()
        {
            _validator = Substitute.For<IValidator<TransactionRequest>>();
            _transactionRepository = Substitute.For<ITransactionRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _sut = new TransactionService(_transactionRepository, _validator,  _accountRepository);
        }

        // Tests 
        [Fact]
        [Trait("Services", "TransactionService")]
        public async Task Transaction_WhenWithdrawReceivesValidValues_ReturnTransactionResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _faker.Finance.Amount();

            var account = new Account(
                userId,
                "12345-7"
            );

            var request = new TransactionRequest(
                    userId,
                    amount,
                    ETransactionType.WithDraw
            );

            _validator.ValidateAsync(Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(account);

            // seta saldo para conta
            account.Deposit(1000);

            // Act
            var result = await _sut.CreateTransactionAsync(request, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.AccountNumber.Should().Be(account.AccountNumber);
            response.Amount.Should().Be(amount);
            response.Type.Should().Be(ETransactionType.WithDraw);
            response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        [Trait("Services", "TransactionService")]
        public async Task CreateTransactionAsync_WhenAccountNotFound_ReturnsAccountNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _faker.Finance.Amount();

            var request = new TransactionRequest(
                    userId,
                    amount,
                    ETransactionType.WithDraw
                );

            _validator.ValidateAsync(Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(userId, Arg.Any<CancellationToken>())
                .ReturnsNull();
            
            // Act
            var result = await _sut.CreateTransactionAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<AccountNotFoundError>();
        }

        [Fact]
        [Trait("Services", "TransationService")]
        public async Task CreateTransactionAsync_WhenBalanceIsInsufficient_ReturnsInsufficientBalanceError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _faker.Finance.Amount();

            var account = new Account(
                userId,
                "12345-7"
            );

            var request = new TransactionRequest(
                    userId,
                    amount,
                    ETransactionType.WithDraw
            );

            _validator.ValidateAsync(Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(account);

            // Act
            var result = await _sut.CreateTransactionAsync(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InsufficientBalanceError>();
        }

        [Fact]
        [Trait("Services","TransactionService")]
        public async Task CreateTransactionAsync_WhenValidDeposit_ReturnsTransactionResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var amount = _faker.Finance.Amount();

            var account = new Account(
                userId,
                "12345-7"
            );

            var request = new TransactionRequest(
                    userId,
                    amount,
                    ETransactionType.Deposit
            );

            _validator.ValidateAsync(Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _accountRepository.GetAccountByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(account);

            // Act
            var result = await _sut.CreateTransactionAsync(request, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.AccountNumber.Should().Be(account.AccountNumber);
            response.Amount.Should().Be(amount);
            response.Type.Should().Be(ETransactionType.Deposit);
            response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }
}
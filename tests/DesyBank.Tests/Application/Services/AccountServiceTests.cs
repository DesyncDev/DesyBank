using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Account;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Application.Services;
using DesyBank.Domain.Models;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DesyBank.Tests.Application.Services
{
    public class AccountServiceTests
    {
        // Mocks
        private readonly IValidator<AccountRequest> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDbRepository _dbRepository;
        private readonly AccountService _sut;

        public AccountServiceTests()
        {
            _validator = Substitute.For<IValidator<AccountRequest>>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _dbRepository = Substitute.For<IDbRepository>();
            _sut = new AccountService(_accountRepository, _userRepository, _validator, _dbRepository);
        }

        // Testes
        [Fact]
        [Trait("Services", "AccountService")]
        public async Task CreateAccount_WithValidData_ReturnsAccountResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountRequest = new AccountRequest(userId);

            _validator.ValidateAsync(accountRequest, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.UserExistsByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(true);

            _accountRepository.AccountExistByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(false);

            _accountRepository.AccountAlreadyExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _sut.CreateAccountAsync(accountRequest, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.AccountNumber.Should().NotBeEmpty();
            response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            
            // Verify
            await _accountRepository.Received(1).AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            await _dbRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "AccountService")]
        public async Task CreateAccount_WhenUserDoesNotExist_ReturnsUnauthenticatedUserError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountRequest = new AccountRequest(userId);
            
            _validator.ValidateAsync(accountRequest, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.UserExistsByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _sut.CreateAccountAsync(accountRequest, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<UnauthenticatedUserError>();

            // Verify
            await _userRepository.Received(1).UserExistsByIdAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "AccountService")]
        public async Task CreateAccount_WhenUserAlreadyHasAccount_ReturnsUserAlreadyHasAnAccountError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountRequest = new AccountRequest(userId);
            
            _validator.ValidateAsync(accountRequest, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.UserExistsByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(true);

            _accountRepository.AccountExistByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _sut.CreateAccountAsync(accountRequest, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<UserAlreadyHasAnAccountError>();

            // Verify
            await _userRepository.Received(1).UserExistsByIdAsync(userId, Arg.Any<CancellationToken>());
            await _accountRepository.Received(1).AccountExistByUserAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "AccountService")]
        public async Task CreateAccount_WhenAccountNumberAlreadyExists_RetriesAndCreatesAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountRequest = new AccountRequest(userId);
            
            _validator.ValidateAsync(accountRequest, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.UserExistsByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(true);

            _accountRepository.AccountExistByUserAsync(userId, Arg.Any<CancellationToken>())
                .Returns(false);

            _accountRepository.AccountAlreadyExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(true, false); 

            // Act
            var result = await _sut.CreateAccountAsync(accountRequest, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();

            // Verify
            await _accountRepository.Received(2).AccountAlreadyExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _accountRepository.Received(1).AddAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
            await _dbRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        // Get Balance

        [Fact]
        [Trait("Services", "AccountService")]
        public async Task GetBalance_WhenAccountExists_ReturnsBalance()
        {
            // Assert
            var userId = Guid.NewGuid();
            var account = new Account(
                userId,
                "12345-6"
            );

            _accountRepository.GetAccountByUserReadAsync(userId, Arg.Any<CancellationToken>())
                .Returns(account);

            // Act
            var result = await _sut.GetBalance(userId, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.Amount.Should().Be(0);
        }

        [Fact]
        [Trait("Services", "AccountService")]
        public async Task GetBalance_WhenAccountNotFound_ReturnsAccountNotFoundError()
        {
            // Assert
            var userId = Guid.NewGuid();

            _accountRepository.GetAccountByUserReadAsync(userId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // Act
            var result = await _sut.GetBalance(userId, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<AccountNotFoundError>();
        }
    }
}
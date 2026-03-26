using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Application.DTOs.User;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Application.Services;
using DesyBank.Domain.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace DesyBank.Tests.Application.Services
{
    public class UserServiceTests
    {
        // Faker
        private readonly Faker _faker = new("pt_BR");

        // Mock
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        
        // Service
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _userService = new UserService(_userRepository, _accountRepository);
        }

        [Fact]
        [Trait("Services", "UserService")]
        public async Task GetMyDataAsync_WhenUserExists_ReturnsUserResponse()
        {
            // Arrange
            var user = CreateValidUser();
            var account = new Account(
                user.Id,
                "123456-7"
            );

            _userRepository.GetUserByIdAsync(user.Id, Arg.Any<CancellationToken>())
                .Returns(user);
            _accountRepository.GetAccountByUserReadAsync(user.Id, Arg.Any<CancellationToken>())
                .Returns(account);

            var expectedUserResponse = new UserResponse(
                user.Id,
                user.FullName,
                account.AccountNumber,
                user.JoinedAt
            );

            // Act
            var result = await _userService.GetMyDataAsync(user.Id, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue(); // É do tipo UserResponse (sucesso)?
    
            var userResponse = result.AsT0; // Pega o valor
            userResponse.Should().Be(expectedUserResponse);

            await _userRepository.Received(1).GetUserByIdAsync(user.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "UserService")]
        public async Task GetMyDataAsync_WhenUserDoesNotExist_ReturnsUserNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // Act
            var result = await _userService.GetMyDataAsync(userId, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();

            var invalidUserResponse = result.AsT1;
            invalidUserResponse.Should().BeOfType<UserNotFoundError>();

             await _userRepository.Received(1).GetUserByIdAsync(userId, Arg.Any<CancellationToken>());
        }

        // Método a auxiliar para criação de usuário válido
        private User CreateValidUser()
        {
            return new User(
                _faker.Name.FullName(),
                _faker.Internet.Email(),
                "exemplo_hash_password"
            );
        }
    }
}
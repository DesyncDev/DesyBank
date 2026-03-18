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
        
        // Service
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _userService = new UserService(_userRepository);
        }

        [Fact]
        [Trait("Services", "UserService")]
        public async Task GetMyDataAsync_WhenUserExists_ReturnsUserResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUserResponse = CreateValidUserResponse();

            _userRepository.GetUserByIdAsync(userId, Arg.Any<CancellationToken>())
                .Returns(expectedUserResponse);

            // Act
            var result = await _userService.GetMyDataAsync(userId, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue(); // É do tipo UserResponse (sucesso)?
    
            var userResponse = result.AsT0; // Pega o valor
            userResponse.Should().Be(expectedUserResponse);

            await _userRepository.Received(1).GetUserByIdAsync(userId, Arg.Any<CancellationToken>());
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
        private UserResponse CreateValidUserResponse()
        {
            return new UserResponse(
                Id: Guid.NewGuid(),
                FullName: _faker.Name.FullName(),
                JoinedAt: DateTime.UtcNow
            );
        }
    }
}
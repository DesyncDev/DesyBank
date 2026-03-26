using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Application.DTOs.Auth;
using DesyBank.Application.DTOs.Auth.Login;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces.Hasher;
using DesyBank.Application.Interfaces.JWT;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Application.Services;
using DesyBank.Domain.Models;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace DesyBank.Tests.Application.Services
{
    public class AuthServiceTests
    {
        // Faker
        private readonly Faker _faker = new("pt_BR");

        // Mock
        private readonly IUserRepository _userRepository;
        private readonly IDbRepository _dbRepository;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _token;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _dbRepository = Substitute.For<IDbRepository>();
            _hasher = Substitute.For<IPasswordHasher>();
            _token = Substitute.For<ITokenService>();
            _registerValidator = Substitute.For<IValidator<RegisterRequest>>();
            _loginValidator = Substitute.For<IValidator<LoginRequest>>();
            _sut = new AuthService(_userRepository, _hasher, _token, _registerValidator, _loginValidator, _dbRepository);
        }

        // Login Tests

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Login_WhenCredentialsAreValid_ShouldReturnSuccessWithTokenEmailAndId()
        {
            // Arrange
            string expectedFullName = _faker.Name.FullName();
            string expectedEmail = _faker.Internet.Email();
            string password = _faker.Internet.Password();
            string expectedFakeHash = "fake_expected_hash";
            string expectedFakeToken = "fake_expected_token";
            
            var user = new User(
                expectedFullName,
                expectedEmail,
                expectedFakeHash
            );

            var loginRequest = new LoginRequest(
                expectedEmail,
                password
            );

            _loginValidator.ValidateAsync(Arg.Any<LoginRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.GetUserByEmailAsync(expectedEmail, Arg.Any<CancellationToken>())
                .Returns(user);

            _hasher.ValidateHash(password, expectedFakeHash)
                .Returns(true);

            _token.GenerateToken(user)
                .Returns(expectedFakeToken);

            // Act
            var result = await _sut.Login(loginRequest, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var loginResponse = result.AsT0;
            loginResponse.Id.Should().Be(user.Id);
            loginResponse.Token.Should().Be(expectedFakeToken);

            await _userRepository.Received(1).GetUserByEmailAsync(expectedEmail, Arg.Any<CancellationToken>());
            _hasher.Received(1).ValidateHash(password, expectedFakeHash);
            _token.Received(1).GenerateToken(user);
        } 

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Login_WhenEmailNotExist_ShouldReturnInvalidCredentialsError()
        {
            // --- ARRANGE ---
            var email = _faker.Internet.Email();
            var loginRequest = new LoginRequest(email, "qualquer_senha");

            _loginValidator.ValidateAsync(Arg.Any<LoginRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
                .ReturnsNull();

            // --- ACT ---
            var result = await _sut.Login(loginRequest, CancellationToken.None);

            // --- ASSERT ---
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<InvalidCredentialsError>();

            await _userRepository.Received(1).GetUserByEmailAsync(email, Arg.Any<CancellationToken>());
            _hasher.DidNotReceiveWithAnyArgs().ValidateHash(Arg.Any<string>(), Arg.Any<string>());
            _token.DidNotReceiveWithAnyArgs().GenerateToken(Arg.Any<User>());
        }

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Login_WhenPasswordIsIncorrect_ShouldReturnInvalidCredentialsError()
        {
            // Arrange
            var fullName = _faker.Name.FullName();
            var email = _faker.Internet.Email();
            var password = "password";
            var expectedHashPassword = "Fake_Password_Hash";
            var loginRequest = new LoginRequest(
                email,
                password
            );
            var user = new User(
                fullName,
                email,
                expectedHashPassword
            );

            _loginValidator.ValidateAsync(Arg.Any<LoginRequest>(), Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.GetUserByEmailAsync(email, Arg.Any<CancellationToken>())
                .Returns(user);

            _hasher.ValidateHash(password, expectedHashPassword)
                .Returns(false);

            // Act
            var result = await _sut.Login(loginRequest, CancellationToken.None);

            // Arrange
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<InvalidCredentialsError>();

            await _userRepository.Received(1).GetUserByEmailAsync(email, Arg.Any<CancellationToken>());
            _hasher.Received(1).ValidateHash(password, expectedHashPassword);
            _token.DidNotReceiveWithAnyArgs().GenerateToken(Arg.Any<User>());
        }

        // Register Tests
        [Fact]
        [Trait("Services", "AuthService")]
        public async Task  Register_WhenCredentialsAreValid_ShouldReturnSuccessWithIdFullNameAndEmail()
        {
            // Arrange
            var expectedFullName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email().Trim().ToLower();
            var password = "password";
            var expectedFakeHash = "fake_hash_password";

            var expectedRequest = new RegisterRequest(
                expectedFullName,
                expectedEmail,
                password
            );

            _registerValidator.ValidateAsync(expectedRequest, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.EmailAlreadyExistsAsync(expectedEmail, Arg.Any<CancellationToken>())
                .Returns(false);

            _hasher.GenerateHash(password).Returns(expectedFakeHash);

            // Act
            var result = await _sut.Register(expectedRequest, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            var response = result.AsT0;
            response.FullName.Should().Be(expectedFullName);
            response.Email.Should().Be(expectedEmail);
            response.Id.Should().NotBe(Guid.Empty);

            await _userRepository.Received(1).AddUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
            await _userRepository.Received(1).AddUserAsync(
                Arg.Is<User>(u => u.Email == expectedEmail && u.FullName == expectedFullName && u.HashPassword == expectedFakeHash),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Register_WhenEmailAlreadyExists_ShouldReturnEmailAlreadyRegisteredError()
        {
            // Arrange
            var expectedFullName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var password = "password";

            var expectedRequest = new RegisterRequest(
                expectedFullName,
                expectedEmail,
                password
            );

            _registerValidator.ValidateAsync(expectedRequest, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            _userRepository.EmailAlreadyExistsAsync(expectedEmail, Arg.Any<CancellationToken>())
                .Returns(true);
            
            // Act
            var result = await _sut.Register(expectedRequest, CancellationToken.None);

            // Arrange
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<EmailAlreadyRegisteredError>();

            await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
            _hasher.DidNotReceive().GenerateHash(Arg.Any<string>());
        }

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Register_WhenNameContainsNumbers_ShouldReturnNameContainsInvalidCharactersError()
        {
            // Arrange
            var invalidName = "Joao 123";
            var email = _faker.Internet.Email();
            var request = new RegisterRequest(invalidName, email, "password");

            _registerValidator.ValidateAsync(request, Arg.Any<CancellationToken>())
                .Returns(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _sut.Register(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<NameContainsInvalidCharactersError>();

            await _userRepository.DidNotReceive().EmailAlreadyExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Services", "AuthService")]
        public async Task Register_WhenRequestIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var request = new RegisterRequest("", "email_invalido", "");
            
            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Email", "E-mail inválido"),
                new("FullName", "Nome é obrigatório")
            };
            var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

            _registerValidator.ValidateAsync(request, Arg.Any<CancellationToken>())
                .Returns(validationResult);

            // Act
            var result = await _sut.Register(request, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            var error = result.AsT1.Should().BeOfType<ValidationError>().Subject;
            error.detail.Should().Contain("E-mail inválido");
            error.detail.Should().Contain("Nome é obrigatório");

            await _userRepository.DidNotReceiveWithAnyArgs().EmailAlreadyExistsAsync(default!, default);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Auth;
using DesyBank.Application.DTOs.Auth.Login;
using DesyBank.Application.Errors;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Hasher;
using DesyBank.Application.Interfaces.JWT;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Enums;
using DesyBank.Domain.Models;
using FluentValidation;
using OneOf;

namespace DesyBank.Application.Services
{
    public class AuthService : IAuthService
    {
        // DI
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _token;

        // Validators
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;

        public AuthService(IUserRepository repository, IPasswordHasher hasher,
         ITokenService token, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
        {
            _repository = repository;
            _hasher = hasher;
            _token = token;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        // Methods
        public async Task<OneOf<LoginResponse, AppError>> Login(LoginRequest loginRequest, CancellationToken ct)
        {
            // Validator
            var validation = await _loginValidator.ValidateAsync(loginRequest, ct);

            // Verifica se usuario existe
            var user = await _repository.GetUserByEmailAsync(loginRequest.Email, ct);

            if (user == null)
                return new InvalidCredentialsError();

            // Verifica se o hash da senha bate com o hash do banco
            var validHash = _hasher.ValidateHash(loginRequest.Password, user.HashPassword);

            if (!validHash)
                return new InvalidCredentialsError();

            // Gera token

            var token = _token.GenerateToken(user);

            return new LoginResponse(
                user.Id,
                token
            );
        }

        public async Task<OneOf<RegisterResponse, AppError>> Register(RegisterRequest registerRequest, CancellationToken ct)
        {
            // Validator
            var validation = await _registerValidator.ValidateAsync(registerRequest, ct);

            if (!validation.IsValid)
            {
                var detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));

                return new ValidationError(
                    detail
                );
            }

            // Verifica se o nome contém apenas letras
            bool onlyLetters = registerRequest.FullName.All(x => char.IsLetter(x) || x == ' ');

            if (!onlyLetters)
                return new NameContainsInvalidCharactersError();

            var emailExist = await _repository.EmailAlreadyExistsAsync(registerRequest.Email, ct);

            if (emailExist)
                return new EmailAlreadyResgisteredError();

            // Cria hash da senha
            var hashPassword = _hasher.GenerateHash(registerRequest.Password);

            // Cria usuario
            var newUser = new User(
                registerRequest.FullName,
                registerRequest.Email,
                hashPassword
            );

            // Persiste
            await _repository.AddUserAsync(newUser, ct);

            // Retorno
            return new RegisterResponse(
                newUser.Id,
                newUser.FullName,
                newUser.Email
            );
        }
    }
}
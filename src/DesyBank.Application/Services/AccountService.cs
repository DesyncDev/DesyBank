using DesyBank.Application.DTOs.Account;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Errors;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using FluentValidation;
using OneOf;

namespace DesyBank.Application.Services
{
    public class AccountService : IAccountService
    {
        // Validator
        private readonly IValidator<AccountRequest> _validator;

        // Repositories
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDbRepository _dbRepository;

        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository,
            IValidator<AccountRequest> validator, IDbRepository dbRepository
        )
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _dbRepository = dbRepository;  
            _validator = validator; 
        }

        // Methods
        public async Task<OneOf<AccountResponse, AppError>> CreateAccountAsync(AccountRequest request, CancellationToken ct)
        {
            // Fluent Validation
            var validation = await _validator.ValidateAsync(request, ct);

            if (!validation.IsValid)
            {
                var detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));

                return new ValidationError(
                    detail
                );
            }

            // Verificamos se o usuario existe
            var userExist = await _userRepository.UserExistsByIdAsync(request.UserId, ct);

            if (!userExist)
                return new UnauthenticatedUserError();

            // Verifica se o usuário já possui conta
            var userAccountExist = await _accountRepository.AccountExistByUserAsync(request.UserId, ct);

            if (userAccountExist)
                return new UserAlreadyHasAnAccountError();

            // Gera e verifica se o numero ja existe
            string accountNumber;
            bool accountNumberExist;

            do
            {
                accountNumber = GenerateAccountNumber();
                accountNumberExist = await _accountRepository.AccountAlreadyExistsAsync(accountNumber, ct); // Verifica SE EXISTE
            }while (accountNumberExist);

            // Cria a conta
            var newAccount = new Account(
                request.UserId,
                accountNumber
            );

            // Persiste
            await _accountRepository.AddAsync(newAccount, ct);
            await _dbRepository.SaveChangesAsync(ct);

            // Retorno
            return new AccountResponse(
                newAccount.AccountNumber,
                newAccount.CreatedAt
            );
        } 

        public async Task<OneOf<OperationRequest, AppError>> GetBalance(Guid userId, CancellationToken ct)
        {
            var account = await _accountRepository.GetAccountByUserReadAsync(userId, ct);

            if (account == null)
                return new AccountNotFoundError();

            return new OperationRequest(account.Balance); 
        }

        // Métodos auxiliares
        private string GenerateAccountNumber()
        {
            Random _random = new();

            var number = _random.Next(10000, 9999999);
            var dv = _random.Next(1, 10);

            string accountNumber = $"{number}-{dv}";
            return accountNumber;
        }  
    }
}
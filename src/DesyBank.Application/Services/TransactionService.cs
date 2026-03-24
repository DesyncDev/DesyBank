using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Errors;
using DesyBank.Application.Errors.ErrorList;
using DesyBank.Application.Interfaces;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Enums;
using DesyBank.Domain.Models;
using FluentValidation;
using OneOf;

namespace DesyBank.Application.Services
{
    public class TransactionService : ITransactionService
    {
        // Validator
        private readonly IValidator<TransactionRequest> _validator;

        // Repositories
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDbRepository _dbRepository;

        public TransactionService(ITransactionRepository repository, IValidator<TransactionRequest> validator,
            IAccountRepository accountRepository, IDbRepository dbRepository
        )
        {
            _transactionRepository = repository;
            _validator = validator;
            _accountRepository = accountRepository;
            _dbRepository = dbRepository;
        }

        // Methods
        public async Task<OneOf<TransactionResponse, AppError>> CreateTransactionAsync(TransactionRequest request, CancellationToken ct)
        {
            // Fluent Validator
            var validation = await _validator.ValidateAsync(request, ct);

            if (!validation.IsValid)
            {
                var detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));

                return new ValidationError(
                    detail
                );
            }

            // Busca a conta via numero
            var account = await _accountRepository.GetAccountByUserAsync(request.UserId, ct);
            
            if (account == null)
                    return new AccountNotFoundError();

            // Verificações de saque
            if (request.Type == ETransactionType.WithDraw)
            {
                if (account.Balance < request.Amount)
                    return new InsufficientBalanceError();

                account.WithDraw(request.Amount);
            }

            if (request.Type == ETransactionType.Deposit)
            {
                account.Deposit(request.Amount);
            }

            // Gera transaction
            var transaction = new Transaction(
                account.Id,
                request.Amount,
                request.Type
            );

            // Persiste
            await _transactionRepository.AddAsync(transaction, ct);
            await _dbRepository.SaveChangesAsync(ct);

            return new TransactionResponse(
                account.AccountNumber,
                request.Amount,
                request.Type,
                DateTime.UtcNow
            );
        }
    }
}
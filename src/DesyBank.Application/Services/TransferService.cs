using DesyBank.Application.DTOs.Transfer;
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
    public class TransferService : ITransferService
    {
        // Repositories
        private readonly ITransferRepository _transferRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDbRepository _dbRepository;

        // Validator
        private readonly IValidator<TransferRequest> _validator;

        public TransferService(ITransferRepository transferRepository,
         IAccountRepository accountRepository, IValidator<TransferRequest> validator,
            ITransactionRepository transactionRepository, IDbRepository dbRepository
        )
        {
            _transferRepository = transferRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _dbRepository = dbRepository;
            _validator = validator;    
        }

        // Methods
        public async Task<OneOf<TransferResponse, AppError>> CreateTransferAsync(TransferRequest request, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(request, ct);

            if (!validation.IsValid)
            {
                var detail = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));

                return new ValidationError(
                    detail
                );
            }
            
            var userAccount = await _accountRepository.GetAccountByUserAsync(request.FromAccountUserId, ct);

            if (userAccount == null)
                return new AccountNotFoundError();

            var toAccount = await _accountRepository.GetAcountByNumberAsync(request.ToAccountNumber, ct);

            if (toAccount == null)
                return new AccountNotFoundError();

            if (toAccount.UserId == userAccount.UserId)
                return new CantTransferForOwnAccountError();

            if (userAccount.Balance < request.Amount)   
                return new InsufficientBalanceError();

            userAccount.TransferTo(toAccount, request.Amount);

            // Gera transfer
            var newTransfer = new Transfer(
                userAccount.Id,
                toAccount.Id,
                request.Amount
            );

            // Gera Transaction
            var debitTransaction = new Transaction(
                userAccount.Id,
                request.Amount,
                ETransactionType.Transfer,
                ETransferType.Debit
            );  

            var creditTransaction = new Transaction(
                toAccount.Id,
                request.Amount,
                ETransactionType.Transfer,
                 ETransferType.Credit
            );

            // Salva os dois
            await _transferRepository.AddAsync(newTransfer, ct);
            await _transactionRepository.AddAsync(debitTransaction, ct);
            await _transactionRepository.AddAsync(creditTransaction, ct);

            await _dbRepository.SaveChangesAsync(ct);

            // Retorno
            return new TransferResponse(
                userAccount.Id,
                toAccount.Id,
                request.Amount,
                DateTime.Now
            );
        }
    }
}
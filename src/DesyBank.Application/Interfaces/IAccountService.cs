using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Account;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Errors;
using OneOf;

namespace DesyBank.Application.Interfaces
{
    public interface IAccountService
    {
        Task<OneOf<AccountResponse, AppError>> CreateAccountAsync(AccountRequest request, CancellationToken ct);
        Task<OneOf<OperationRequest, AppError>> GetBalance(Guid userId, CancellationToken ct);
    }
}
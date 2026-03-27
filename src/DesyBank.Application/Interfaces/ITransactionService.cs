using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DesyBank.Application.DTOs.Paged;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Errors;
using OneOf;

namespace DesyBank.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<OneOf<TransactionResponse, AppError>> CreateTransactionAsync(TransactionRequest request, CancellationToken ct);
        Task<OneOf<PagedResponse<TransactionResponse>, AppError>> GetPagedTransactionsAsync(Guid userId, int pages, int length, CancellationToken ct);
    }
}
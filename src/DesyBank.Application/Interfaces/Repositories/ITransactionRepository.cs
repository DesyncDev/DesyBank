using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Domain.Models;

namespace DesyBank.Application.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken ct);
        Task<(List<Transaction> Items, int Total)> GetPagedTransactionsAsync(int pages, int length, CancellationToken ct);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;

namespace DesyBank.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account, CancellationToken ct);
        Task<Account?> GetAccountByUserAsync(Guid userId, CancellationToken ct);
        Task<Account?> GetAccountByUserReadAsync(Guid userId, CancellationToken ct);
        Task<Account?> GetAcountByNumberAsync(string accountNumber, CancellationToken ct);
        Task<bool> AccountExistByUserAsync(Guid userId, CancellationToken ct);
        Task<bool> AccountAlreadyExistsAsync(string accountNumber, CancellationToken ct);
    }
}
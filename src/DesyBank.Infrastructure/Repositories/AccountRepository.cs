using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using DesyBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DesyBank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository 
    {
        // Database
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methods
        public async Task AddAsync(Account account, CancellationToken ct)
        {
            _context.Accounts.Add(account);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);

        public async Task<Account?> GetAccountByUserAsync(Guid userId, CancellationToken ct)
        => await _context.Accounts
        .Where(an => an.UserId == userId)
        .FirstOrDefaultAsync(ct);

        public async Task<Account?> GetAccountByUserReadAsync(Guid userId, CancellationToken ct)
        => await _context.Accounts
        .AsNoTracking()
        .Where(an => an.UserId == userId)
        .FirstOrDefaultAsync(ct);

        public async Task<Account?> GetAcountByNumberAsync(string accountNumber, CancellationToken ct)
        => await _context.Accounts
        .Where(an => an.AccountNumber == accountNumber)
        .FirstOrDefaultAsync(ct);

        // Validation Methods
        public async Task<bool> AccountAlreadyExistsAsync(string accountNumber, CancellationToken ct)
        => await _context.Accounts
        .AsNoTracking()
        .AnyAsync(an => an.AccountNumber == accountNumber, ct);

        public async Task<bool> AccountExistByUserAsync(Guid userId, CancellationToken ct)
        => await _context.Accounts
        .AsNoTracking()
        .AnyAsync(u => u.UserId == userId, ct);
    }
}
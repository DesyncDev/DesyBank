using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transaction;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using DesyBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DesyBank.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        // Database
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methods
        public async Task AddAsync(Transaction transaction, CancellationToken ct)
        {
            _context.Transactions.Add(transaction);
        }

        public async Task<(List<Transaction> Items, int Total)> GetPagedTransactionsAsync(int pages, int length, CancellationToken ct)
        {
            var query = _context.Transactions
            .AsNoTracking();

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pages - 1) * length)
                .Take(length)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
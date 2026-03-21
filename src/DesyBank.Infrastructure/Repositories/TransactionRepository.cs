using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using DesyBank.Infrastructure.Data;

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
            await _context.SaveChangesAsync(ct);
        }
    }
}
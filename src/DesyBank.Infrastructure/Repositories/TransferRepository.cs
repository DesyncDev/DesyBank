using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Domain.Models;
using DesyBank.Infrastructure.Data;

namespace DesyBank.Infrastructure.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        // Database
        private readonly AppDbContext _context;

        public TransferRepository(AppDbContext context)
        {
            _context = context;
        }

        // Métodos
        public async Task AddAsync(Transfer transfer, CancellationToken ct)
        {
            _context.Transfers.Add(transfer);
        }
    }
}
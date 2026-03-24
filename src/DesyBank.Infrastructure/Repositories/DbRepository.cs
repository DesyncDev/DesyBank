using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.Interfaces.Repositories;
using DesyBank.Infrastructure.Data;

namespace DesyBank.Infrastructure.Repositories
{
    public class DbRepository : IDbRepository
    {
        // Database
        private readonly AppDbContext _context;

        public DbRepository(AppDbContext context)
        {
            _context = context;
        }

        // Métodos
        public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
    }
}
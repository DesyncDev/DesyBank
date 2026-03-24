using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.Interfaces.Repositories
{
    public interface IDbRepository
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
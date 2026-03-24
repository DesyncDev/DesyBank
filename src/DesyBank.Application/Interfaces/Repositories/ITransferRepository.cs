using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;

namespace DesyBank.Application.Interfaces.Repositories
{
    public interface ITransferRepository
    {
        Task AddAsync(Transfer transfer, CancellationToken ct);
    }
}
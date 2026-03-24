using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.DTOs.Transfer;
using DesyBank.Application.Errors;
using OneOf;

namespace DesyBank.Application.Interfaces
{
    public interface ITransferService
    {
        Task<OneOf<TransferResponse, AppError>> CreateTransferAsync(TransferRequest request, CancellationToken ct);
    }
}
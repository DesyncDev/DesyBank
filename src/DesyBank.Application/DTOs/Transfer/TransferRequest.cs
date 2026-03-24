using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.Transfer
{
    public sealed record TransferRequest
    (
        Guid FromAccountUserId,
        string ToAccountNumber,
        decimal Amount
    );
}
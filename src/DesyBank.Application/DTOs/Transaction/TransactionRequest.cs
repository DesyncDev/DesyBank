using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.DTOs.Transaction
{
    public sealed record TransactionRequest
    (
        Guid UserId,
        decimal Amount,
        ETransactionType Type
    );
}
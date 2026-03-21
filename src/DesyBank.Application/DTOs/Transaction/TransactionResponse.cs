using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.DTOs.Transaction
{
    public sealed record TransactionResponse
    (
        string AccountNumber,
        decimal Amount,
        ETransactionType Type,
        DateTime CreatedAt
    );
}
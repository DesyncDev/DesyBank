using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Domain.Models
{
    public class Transaction
    {
        // Constructor
        public Transaction(Guid accountId, decimal amount, ETransactionType type, ETransferType? transferType)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Type = type;
            TransferType = transferType;
            CreatedAt = DateTime.UtcNow;
        }

        // Properties
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Account Account { get; private set; } = null!;
        public decimal Amount { get; init; }
        public ETransactionType Type { get; private set; }
        public ETransferType? TransferType { get; private set; }
        public DateTime CreatedAt { get; init; }
    }
}
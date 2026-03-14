using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Domain.Models
{
    public class Account
    {
        // Constructor
        public Account(Guid userId, string accountNumber)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            AccountNumber = accountNumber;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        // Properties
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public User User { get; private set; } = null!;
        public string AccountNumber { get; init; } = string.Empty;
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; init; }

        // Lists
        private List<Transaction> _transactions = new();
        private readonly List<Transfer> _sentTransfers = new();
        private readonly List<Transfer> _receivedTransfers = new();

        // Collections
        public IReadOnlyCollection<Transaction> Transactions => _transactions;
        public IReadOnlyCollection<Transfer> SentTransfers => _sentTransfers;
        public IReadOnlyCollection<Transfer> ReceivedTransfers => _receivedTransfers;
    }
}
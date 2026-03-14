using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Domain.Models
{
    public class Transfer
    {
        // Constructor
        public Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            Id = Guid.NewGuid();
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Amount = amount;
            CreatedAt = DateTime.UtcNow;
        }
        
        // Properties
        public Guid Id { get; init; }
        public Guid FromAccountId { get; init; }
        public Account FromAccount { get; private set; } = null!;
        public Guid ToAccountId { get; init; }
        public Account ToAccount { get; private set; } = null!;
        public decimal Amount { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
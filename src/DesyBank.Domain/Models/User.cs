using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace DesyBank.Domain.Models
{
    public class User
    {
        // Constructor
        public User(string fullName, string email, string hashPassword)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            HashPassword = hashPassword;
            IsBlocked = false;
            JoinedAt = DateTime.UtcNow;
        }

        // Properties
        public Guid Id { get; init; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string HashPassword { get; private set; } = string.Empty;
        public bool IsBlocked { get; private set; }
        public DateTime JoinedAt { get; init; }
    }
}
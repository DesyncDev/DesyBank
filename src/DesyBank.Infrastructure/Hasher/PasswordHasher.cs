using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Application.Interfaces.Hasher;

namespace DesyBank.Infrastructure.Hasher
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateHash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

        public bool ValidateHash(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
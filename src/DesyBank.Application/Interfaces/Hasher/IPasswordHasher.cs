using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.Interfaces.Hasher
{
    public interface IPasswordHasher
    {
        string GenerateHash(string password);
        bool ValidateHash(string password, string hash);
    }
}
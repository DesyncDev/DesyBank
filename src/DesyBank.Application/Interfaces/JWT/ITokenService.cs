using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;

namespace DesyBank.Application.Interfaces.JWT
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
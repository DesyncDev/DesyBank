using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.Account
{
    public sealed record AccountResponse
    (
        string AccountNumber,
        DateTime CreatedAt
    );
}
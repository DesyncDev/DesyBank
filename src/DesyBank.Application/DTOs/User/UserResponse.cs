using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.User
{
    public sealed record UserResponse
    (
        Guid Id,
        string FullName,
        string AccountNumber,
        DateTime JoinedAt
    );
}
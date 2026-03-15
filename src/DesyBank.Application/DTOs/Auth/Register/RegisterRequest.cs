using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.Auth
{
    public sealed record RegisterRequest
    (
        string FullName,
        string Email,
        string Password
    );
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesyBank.Application.DTOs.User
{
    public sealed record UserRequest
    (
        string FullName,
        string Email,
        string HashPassword
    );
}
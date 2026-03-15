using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.Errors
{
    public record AppError(string detail, EErrorType type, string errorName);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.Errors.ErrorList
{
    public record AccountNotFoundError() : AppError("Account not found.", EErrorType.NotFoundError, nameof(AccountNotFoundError));
}
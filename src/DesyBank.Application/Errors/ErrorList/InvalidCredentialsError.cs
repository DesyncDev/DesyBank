using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.Errors.ErrorList
{
    public record InvalidCredentialsError() : AppError("Invalid credentials.", EErrorType.UnauthorizedError, nameof(InvalidCredentialsError));
}
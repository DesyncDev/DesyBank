using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Enums;

namespace DesyBank.Application.Errors.ErrorList
{
    public record CantTransferForOwnAccountError() : AppError("Can't transfer for own account.", EErrorType.BusinessRuleError, nameof(CantTransferForOwnAccountError));
}